using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace formfiller
{
    class BookmarkLookup
    {
        public static string Value(Dictionary<string, string> fields, string bookmarkName)
        {
            DateTime date;
            String text;

            switch (bookmarkName)
            {
                case "Petitioner1":
                case "Petitioner2":
                    return fields.TryGetValue("petitioner-name", out text) ? text : null;
                case "Respondent1":
                case "Respondent2":
                    return fields.TryGetValue("respondent-name", out text) ? text : null;
                case "BirthdatePet1":
                    return fields.TryGetValue("petitioner-birth-date", out text) ? text.Substring(0, 10) : null;
                case "BirthdateRep2":
                    return fields.TryGetValue("respondent-birth-date", out text) ? text.Substring(0, 10) : null;
                case "ResidencePet":
                    return fields.TryGetValue("petitioner-location", out text) ? text : null;
                case "ResidenceResp":
                    return fields.TryGetValue("respondent-location", out text) ? text : null;
                case "dateofmarriage":
                    return fields.TryGetValue("marriage-date", out text) ? text.Substring(0, 10) : null;
                case "citystatemarriage":
                    return fields.TryGetValue("marriage-location", out text) ? text : null;
                /*case "sharedkidsyes":
                    return jdata.Value<bool>("children") ? " " : "X";
                case "sharedkidsno":
                    return jdata.Value<bool>("children") ? "X" : " ";*/
                case "sharedkidname1":
                    return fields.TryGetValue("dual-children-0-name", out text) ? text : null;
                case "sharedkidage1":
                    if (fields.TryGetValue("dual-children-0-dob", out text) && DateTime.TryParse(text, out date))
                        return Math.Floor((DateTime.Now - date).TotalDays / 365).ToString();
                    else
                        return "";
                case "sharedkidname2":
                    return fields.TryGetValue("dual-children-1-name", out text) ? text : null;
                case "sharedkidage2":
                    if (fields.TryGetValue("dual-children-1-dob", out text) && DateTime.TryParse(text, out date))
                        return Math.Floor((DateTime.Now - date).TotalDays / 365).ToString();
                    else
                        return "";
                case "Name1":
                case "petkidname1":
                case "petkidage1":
                case "repkidname1":
                case "repkidage1":
                case "marriagebrokenyesno":
                case "separatedyesno":
                case "dateofseparation":
                case "datemovedyesno":
                case "datepropertydivided":
                case "datepetfiled":
                case "dateagreedsep":
                case "dateother":
                case "yesjurisdiction":
                case "respliveswa":
                case "bothlivedwapetremains":
                case "petmilitary":
                case "petandrepconceivechiildwa":
                case "otherjurisreason":
                case "nojurisdiction":
                case "propertyyesno":
                case "courtdivideproperty":
                case "petsreconproperty":
                case "petgets":
                case "respondentgets":
                case "Othergetsproperty":
                default:
                    return null;
            }
        }
    }
}
