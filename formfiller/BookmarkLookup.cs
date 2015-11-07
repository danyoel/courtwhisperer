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
        public static string Value(JObject data, string bookmarkName)
        {
            switch (bookmarkName)
            {
                case "Petitioner1":
                case "Petitioner2":
                    return String.Format("{0} {1}", data["petitioner"]["firstName"], data["petitioner"]["lastName"]);
                case "Respondent1":
                case "Respondent2":
                    return String.Format("{0} {1}", data["respondent"]["firstName"], data["respondent"]["lastName"]);
                case "BirthdatePet1":
                    return data["petitioner"]["birthDate"].Value<string>().Substring(0, 10);
                case "BirthdateRep2":
                    return data["respondent"]["birthDate"].Value<string>().Substring(0, 10);
                case "ResidencePet":
                    return String.Format("{0}, {1}", data["petitioner"]["lastKnownCounty"], data["petitioner"]["lastKnownState"]);
                case "ResidenceResp":
                    return String.Format("{0}, {1}", data["respondent"]["lastKnownCounty"], data["respondent"]["lastKnownState"]);
                case "dateofmarriage":
                    return data["dateMarried"].Value<string>().Substring(0, 10);
                case "citystatemarriage":
                    return String.Format("{0}, {1}", data["cityMarried"], data["stateMarried"]);

                case "kidsyesno":
                case "sharedkidname1":
                case "sharedkidage1":
                case "sharedkidname2":
                case "sharedkidage2":
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
