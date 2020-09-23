using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml;
using System.Xml.Linq;
using TranslatorService.Models;

namespace TranslatorService.Controllers
{
    public class TranslateController : ApiController
    {

        // GET: api/Translate
        [Route("api/Translate"),HttpGet]
        [EnableCors(origins:"*",headers:"*" , methods:"*")]
        public IHttpActionResult Translate(string text, string lan)
        {
            Translation translation = CheckTranslation(text, lan);
            if (translation != null)
                return Ok(translation.To);

            // else: Translate Api call
            string translatedText = "You are awesome :)"; //api returned this
            AddTranslation(text, lan, translatedText);
            return Ok(translatedText);                           


        }

        private void AddTranslation(string text, string lan, string translatedText)
        {
            var xmlFilePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Results.xml");
            if (File.Exists(xmlFilePath))
            {
                XDocument doc = XDocument.Load(xmlFilePath);               
                XElement translations = doc.Element("Translations");
                int nextId = translations.Nodes().Count() + 1;
                translations.Add(new XElement("Translation",
                           new XAttribute("Timestamp", DateTime.Now.ToString()),
                           new XAttribute("Id", nextId),
                           new XElement("Language", lan),
                           new XElement("From", text),
                           new XElement("To", translatedText)));
                doc.Save(xmlFilePath);
            }

        }

        private Translation CheckTranslation(string text, string lan)
        {
            var xmlFilePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Results.xml");

            List<Translation> translationsList = new List<Translation>();
            if (File.Exists(xmlFilePath))
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                List<XNode> xNodes = doc.DescendantNodes().ToList();

                foreach (XNode node in xNodes)
                {
                    XElement element = node as XElement;
                    if (element != null)
                    {
                        if (element.Name == "Language")     //kad nadjem Language uzimam njega i sledeca 2 (from i to) , ostali su nebitni
                        {
                            XElement fromElement = element.NextNode as XElement;
                            XElement toElement = fromElement.NextNode as XElement;
                            Translation translation = new Translation()
                            {
                                Language = element.Value,
                                From = fromElement.Value,
                                To = toElement.Value
                            };
                            translationsList.Add(translation);
                        }

                    }
                }

            }

            return translationsList.Where(x => x.Language.ToLower() == lan.ToLower() && x.From.ToLower() == text.ToLower()).FirstOrDefault();

        }

    }
}
