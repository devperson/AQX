using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Domain.Responses;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
    [DataContract]
    public class DismissAlertRequest : ApiRequestForPivotPrograms<DismissAlertRequestBody>
    {
        public const string RequestKey = "Req.AckAlerts";

        public DismissAlertRequest(string alertId) : base(RequestKey)
        {
            this.Body = new DismissAlertRequestBody(new List<string>() { alertId });
        }

        public DismissAlertRequest(IEnumerable<string> alertIds) : base(RequestKey)
        {
            this.Body = new DismissAlertRequestBody(alertIds);
        }
    }

    [DataContract]
    public class DismissAlertRequestBody
    {
        [DataMember(Name = "Alerts")]
        public Alerts Alerts { get; set; }

        public DismissAlertRequestBody(IEnumerable<string> alertIds)
        {
            Alerts alerts = new Alerts();
            foreach (string id in alertIds)
            {
                alerts.Add(id, new object());
            }
            this.Alerts = alerts;
        }
    }

    [DataContract]
    public class Alerts : ItemsDictionary<object>
    {
        public static string Id { get; internal set; }
    }
}
