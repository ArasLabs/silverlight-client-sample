using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Net;
using System.Net.Browser;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Threading;
using Encoding = System.Text.Encoding;
using System.Windows.Browser;


namespace Sample_Silverlight_Application
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        public bool m_IsEditMode;
        public static XDocument MyCache;
        public static XDocument MyLists;
        public static XElement MyCacheItem;
        public static string myName;
        public static string myId;
        public static SynchronizationContext SCTEMP = null;

        public MainPage()
        {
            InitializeComponent();
        }


        public static String ConvertPasswordToMD5(String plainPasswd)
        {
            string MD5Passwd = "";
            byte[] result = MD5Core.GetHash(plainPasswd);
            MD5Passwd = BinaryToHex(result);
            return (MD5Passwd.ToLower());
        }

        // Use this function to convert your MD5
        // hash 16 bytes array to 32 hexadecimals string.
        // Note: This code taken from www.gotdotnet.com - Topic: Function to convert your MD5 16 byte hash to 32 hexadecimals 
        public static String BinaryToHex(byte[] BinaryArray)
        {
            string result = "";
            long lowerByte;
            long upperByte;

            foreach (Byte singleByte in BinaryArray)
            {
                lowerByte = singleByte & 15;
                upperByte = singleByte >> 4;

                result += NumberToHex(upperByte);
                result += NumberToHex(lowerByte);
            }
            return result;
        }

        // Convert the number to hexadecimal
        // Note: This code taken from www.gotdotnet.com - Topic: Function to convert your MD5 16 byte hash to 32 hexadecimals 
        private static char NumberToHex(long Number)
        {
            if (Number > 9)
                return Convert.ToChar(65 + (Number - 10));
            else
                return Convert.ToChar(48 + Number);
        }

        private String newID()
        {
            string id = Guid.NewGuid().ToString("N").ToUpper();
            return id;
        }

        //Login Tab and Sync
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // test the Login by sending a  ValidateUser to the Innovator Server 

            //LoginButton.IsEnabled = false;

            CallContext ValidateUser = new CallContext();

            ValidateUser.Action = "ValidateUser";
            ValidateUser.SC = SynchronizationContext.Current;
            ValidateUser.CallBack = (SendOrPostCallback)ExecuteLogin;
            ValidateUser.inDom = XDocument.Parse("<Item type='User' action='get'/>");

            CallAction(ValidateUser);

        }
        private void ExecuteLogin(object cc)
        {
            CallContext Context = (CallContext)cc;


            XElement ErrNode = Context.outDom.XPathSelectElement("//SOAP-ENV:Fault", Context.outDom.Root.CreateNavigator());
            if (ErrNode != null)
            {
                string errorDetail = ErrNode.Element("faultstring").Value;
                FEEDBACKMSG.Text = errorDetail;
                FEEDBACK.Visibility = System.Windows.Visibility.Visible;
                return;
            }

            FEEDBACKMSG.Text = "Logon Successful - running query";
            FEEDBACK.Visibility = System.Windows.Visibility.Visible;
           
            // run the sample query 
            queryData();

 
        }

        private void queryData()
        {
            CallContext dataContext = new CallContext();
            // Grab SynchronizationContext while on UI Thread   
            dataContext.SC = SynchronizationContext.Current;
            dataContext.CallBack = (SendOrPostCallback)syncDataCallBack;
            dataContext.inDom = XDocument.Parse("<Item type='User' select='first_name, last_name, telephone, email' action='get'></Item>");
            CallAction(dataContext);
        }
        private void syncDataCallBack(object cc)
        {
            CallContext Context = (CallContext)cc;

            XElement ErrNode = Context.outDom.XPathSelectElement("//SOAP-ENV:Fault", Context.outDom.Root.CreateNavigator());
            if (ErrNode != null)
            {
                string errorDetail = ErrNode.Element("faultstring").Value;
                FEEDBACKMSG.Text = "Innovator Server Error:" + errorDetail;
                FEEDBACK.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            MyCache = XDocument.Parse(Context.outDom.ToString());

            FEEDBACKMSG.Text = "";
            FEEDBACK.Visibility = System.Windows.Visibility.Collapsed;

            UserGrid.SelectedItem = null;
            SetBindings();
        }
        //functions for communicating with myInnovator
        public partial class CallContext
        {
            public String User;
            public String Password;
            public String Database;
            public String BaseURL;
            public String UserID;
            public System.Xml.Linq.XDocument inDom;
            public System.Xml.Linq.XDocument outDom;
            public String Action;
            public SynchronizationContext SC;
            public SendOrPostCallback CallBack;
            public IAsyncResult ar;
            public HttpWebRequest request;
            public CallContext()
            {
                this.User = "";
                this.Password = "";
                this.Database = "";
                this.BaseURL = "";
                this.UserID = "";
                Action = "ApplyItem";
                inDom = System.Xml.Linq.XDocument.Parse("<Item/>");
                outDom = System.Xml.Linq.XDocument.Parse("<Item/>");
            }
        }
        public void CallAction(CallContext cc)
        {
            // Create the request object  
            System.Uri myUrl = new System.Uri(Server.Text + "/Server/InnovatorServer.aspx");
            HttpWebRequest request = WebRequest.Create(myUrl) as HttpWebRequest;

            request.Method = "POST";
            request.Headers["SOAPAction"] = cc.Action;
            request.ContentType = "text/xml";
            request.Headers["AUTHUSER"] = Login_Name.Text;
            request.Headers["AUTHPASSWORD"] = ConvertPasswordToMD5(Password.Password);
            request.Headers["DATABASE"] = Database.Text;

            cc.request = request;

            //  grab the Thread id while we are still running on the UI thread
            // this is a temp fix until we figure out why the CallingContext is getting stepped on
            SCTEMP = SynchronizationContext.Current;


            // need to make a asynch call just to get the output stream.     makes no sense, but this is SL
            IAsyncResult asyncResult = request.BeginGetRequestStream(new AsyncCallback(RequestStreamCallback), cc);
        }
        private static void RequestStreamCallback(IAsyncResult ar) // this function will write the InDom out to the requestStrea,
        {
            CallContext cc = (CallContext)ar.AsyncState;
            SendOrPostCallback cb = cc.CallBack;

            HttpWebRequest request = cc.request as HttpWebRequest;

            request.ContentType = "text/xml";
            Stream requestStream = request.EndGetRequestStream(ar);
            StreamWriter streamWriter = new StreamWriter(requestStream);
            streamWriter.Write(cc.inDom.ToString());  //    later will come from the InDom on the Call Context
            streamWriter.Close();
            // Make async call for response.  Callback will be called on a background thread.  
            request.BeginGetResponse(new AsyncCallback(ResponseCallback), cc);
        }
        private static void ResponseCallback(IAsyncResult ar)
        {
            CallContext cc = (CallContext)ar.AsyncState;
            SynchronizationContext syncContext = cc.SC;
            syncContext = SCTEMP;

            HttpWebRequest request = (HttpWebRequest)cc.request;
            HttpWebResponse response = null;
            // ADD A TRY/CATCH HERE TO HANDLE THE ERROR OF WRONG URL
            try
            {
                response = (HttpWebResponse)request.EndGetResponse(ar);
            }
            catch (Exception ex)
            {
                // Invoke the original callback function the user wanted,  back onto the UI thread  
                string ErrXml = "<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/'><SOAP-ENV:Body><SOAP-ENV:Fault xmlns:af='http://www.aras.com/InnovatorFault'><faultcode>SOAP-ENV:Server</faultcode><faultstring>" + ex.Message + "</faultstring><detail><af:legacy_detail><![CDATA[" + ex.InnerException + "]]></af:legacy_detail><af:exception message='" + ex.Message + "' type='Aras.Server.Core.InnovatorServerException' /></detail></SOAP-ENV:Fault></SOAP-ENV:Body></SOAP-ENV:Envelope>";
                cc.outDom = System.Xml.Linq.XDocument.Parse(ErrXml);
                syncContext.Post(cc.CallBack, cc);
                return;
            }

            string resultString;

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }

            // Invoke the original callback function the user wanted,  back onto the UI thread  
            cc.outDom = System.Xml.Linq.XDocument.Parse(resultString);
            syncContext.Post(cc.CallBack, cc);
        }

        public class UserClass
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }

        }
        public void SetBindings()
        {
            var myData = from info in MyCache.Root.XPathSelectElements("//Item[@type='User']")
                         select new UserClass
                         {
                             FirstName = Convert.ToString(info.Element("first_name").Value),
                             LastName = Convert.ToString(info.Element("last_name").Value),
                             Email = Convert.ToString(info.Element("email").Value),
                             Phone = Convert.ToString(info.Element("telephone").Value),
                         };
            UserGrid.ItemsSource = myData;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            HtmlPage.Window.Invoke("Close");
        }
    }
}