using CERRS.MCR.JournalInterface.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CERRS.MCR.JournalInterface
{
    class MCR
    {
        public Uri mcrURI { get; set; }
        public MCR()
        {

        }
        public MCR(IOrganizationService orgSvc)
        {
            mcrURI = getMCRURI(orgSvc);
        }
        private Uri getMCRURI(IOrganizationService orgSvc)
        {
            Uri mcrURI = null;

            XRMController xrm = new XRMController(orgSvc);
            mcrURI = new Uri(xrm.getMCRURIString());

            return mcrURI;

        }

        public string postToMCR(string postRequest, string postResource)
        {
            try
            {
                //var mcrURI1 = "https://mcr-proto-cont.dev.cognosante.cc/";
                Uri journalURI = new Uri(mcrURI + postResource);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(journalURI);

                request.Method = "POST";
                request.Accept = "*/*";
                request.ContentType = "application/json";
                request.Headers.Add("x-cognosante-authentication", "6d5d56f7cf494134a96cc6910549341a");
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = postRequest;

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            }
            catch (WebException e)
            {

                using (var streamReader = new StreamReader(e.Response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;

                };
            }

        }

        public HICSData getERR1095Data(string postResource, string id)
        {            
            try
            {
                Uri journalURI = new Uri(mcrURI + postResource + id);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(journalURI);

                request.Method = "GET";
                request.Accept = "*/*";
                request.ContentType = "application/json";
                request.Headers.Add("x-cognosante-authentication", "6d5d56f7cf494134a96cc6910549341a");

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    HICSData objResult = JsonConvert.DeserializeObject<HICSData>(result);

                    return objResult;
                }
            }
            catch (Exception e)
            {
                HICSData objResult = new HICSData();
                objResult.Applicationid = e.Message;
                return objResult;
            }

        }

    }
}
