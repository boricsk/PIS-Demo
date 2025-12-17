using ProdInfoSys.Models.NonRelationalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Classes
{
    public class BuildEmail
    {
        private string _emailHead = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset=""UTF-8"">
            <title>Nyitott feladatok</title>
            <style>
                table {
                    border-collapse: collapse;
                    width: 100%;
                    font-family: Consolas, sans-serif;
                    font-size: 12px;
                }
                th, td {
                    border: 1px solid #999;
                    padding: 8px;
                    text-align: left;
                }
                th {
                    background-color: #eee;
                }
                .container {
                    border: 3px solid red;
                    padding: 15px;
                }
                .head {
                    border-collapse: collapse;
                    width: 100%;
                    font-family: Consolas, sans-serif;
                    font-size: 12px;
                }
            </style>
        </head>
        <body>
            <div class=""head"">
                <h4>Tisztelt Címzett!</h4>
                <p>A QRQC nyitott feladatai az alábbiak:</p>
            </div><hr>
            <div>
                <h4>Nyitott feladatok</h4>
                <table>
                    <thead>
                        <tr>
                            <th>LÉTREHOZÁS DÁTUM</th>
                            <th>FELELŐS</th>
                            <th>HATÁRIDŐ</th>
                            <th>FELADAT</th>
                            <th>MEGJEGYZÉS</th>                                
                        </tr>
                    </thead>
                    <tbody>
        ";

        private string _leaderEmailHead = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset=""UTF-8"">
            <title>Terv összegzés</title>
            <style>
                table {
                    border-collapse: collapse;
                    width: 100%;
                    font-family: Consolas, sans-serif;
                    font-size: 12px;
                }
                th, td {
                    border: 1px solid #999;
                    padding: 8px;
                    text-align: left;
                }
                th {
                    background-color: #eee;
                }
                .container {
                    border: 3px solid red;
                    padding: 15px;
                }
                .head {
                    border-collapse: collapse;
                    width: 100%;
                    font-family: Consolas, sans-serif;
                    font-size: 12px;
                }
            </style>
        </head>
        <body>
            <div class=""head"">
                <h4>Tisztelt Címzett!</h4>
                <p>A gyátási és értékesítési terv részletei az alábbiak:</p>
            </div><hr>
            <div>                
                <table>
                    <thead>
                        <tr>
                            <th>TÉTEL</th>
                            <th>ÉRTÉK</th>
                        </tr>
                    </thead>
                    <tbody>
        ";

        private string _testEmail = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset=""UTF-8"">
            <title>Terv összegzés</title>
            <style>
                table {
                    border-collapse: collapse;
                    width: 100%;
                    font-family: Consolas, sans-serif;
                    font-size: 12px;
                }
                th, td {
                    border: 1px solid #999;
                    padding: 8px;
                    text-align: left;
                }
                th {
                    background-color: #eee;
                }
                .container {
                    border: 3px solid red;
                    padding: 15px;
                }
                .head {
                    border-collapse: collapse;
                    width: 100%;
                    font-family: Consolas, sans-serif;
                    font-size: 12px;
                }
                .paragraph {
                    font-family: Consolas, sans-serif;
                    font-size: 12px;
                }
            </style>
        </head>
        <body>
            <div class=""head"">
                <h4>Test Email from Production Information System</h4>                
            </div><hr>
            <div class=""paragraph"">                

        ";

        private string _testEmailTail = @"
            </div>
        </body>
        </html>
        ";

        private string _emailTail = @"
                    </tbody>
                </table>
            </div>
        </body>
        </html>
        ";

        private List<MeetingMinutes> _memos = new List<MeetingMinutes>();
        private string _SamplePrice;
        private string _PlanPrice;
        private string _PlanDCPrice;
        private string _MaterialCost;
        private string _MaterialCostRepack;
        private string _MaterialCostKft;
        private string _TtlPaln;
        private string _SalesPlan;
        private string _SalesPlanDC;
        private string _DcMovement;
        private string _testEmailAddr;


        public BuildEmail(List<MeetingMinutes> meetingMinutes)
        {
            _memos = meetingMinutes;
        }
        public BuildEmail(string testEmailAddr)
        {
            _testEmailAddr = testEmailAddr;
        }
        public BuildEmail(
            string SamplePrice,
            string PlanPrice,
            string PlanDCPrice,
            string MaterialCost,
            string MaterialCostRepack,
            string MaterialCostKft,
            string TtlPaln,
            string SalesPlan,
            string SalesPlanDC,
            string DcMovement
            )
        {
            _SamplePrice = SamplePrice;
            _PlanPrice = PlanPrice;
            _PlanDCPrice = PlanDCPrice;
            _MaterialCost = MaterialCost;
            _MaterialCostKft = MaterialCostKft;
            _MaterialCostRepack = MaterialCostRepack;
            _TtlPaln = TtlPaln;
            _SalesPlan = SalesPlan;
            _SalesPlanDC = SalesPlanDC;
            _DcMovement = DcMovement;
        }
        public string BuidEmailMessage()
        {
            StringBuilder message = new StringBuilder();
            message.Append(_emailHead);
            foreach (var item in _memos)
            {
                message.Append("<tr>");
                message.Append($"<td style=\"width: 110px;\">{DateOnly.FromDateTime(item.date)}</td>");
                message.Append($"<td>{item.pic}</td>");
                message.Append($"<td style=\"width: 110px;\">{DateOnly.FromDateTime(item.deadline)}</td>");
                message.Append($"<td>{item.task}</td>");
                message.Append($"<td>{item.comment}</td>");
                message.Append("</tr>");
            }
            message.Append(_emailTail);

            return message.ToString();
        }

        public string BuidLeaderEmailMessage()
        {
            StringBuilder message = new StringBuilder();
            message.Append(_leaderEmailHead);

            message.Append("<tr>");
            message.Append($"<td>Minta</td>");
            message.Append($"<td>{_SamplePrice}</td>");
            message.Append("</tr>");

            message.Append("<tr>");
            message.Append($"<td>Kimeneti terv</td>");
            message.Append($"<td>{_PlanPrice}</td>");
            message.Append("</tr>");

            message.Append("<tr>");
            message.Append($"<td>Kimeneti terv DC</td>");
            message.Append($"<td>{_PlanDCPrice}</td>");
            message.Append("</tr>");

            message.Append("<tr>");
            message.Append($"<td>Anyagköltség (TTL)</td>");
            message.Append($"<td>{_MaterialCost}</td>");
            message.Append("</tr>");

            message.Append("<tr>");
            message.Append($"<td>Anyagköltség (Repack)</td>");
            message.Append($"<td>{_MaterialCostRepack}</td>");
            message.Append("</tr>");

            message.Append("<tr>");
            message.Append($"<td>Anyagköltség (KFT)</td>");
            message.Append($"<td>{_MaterialCostKft}</td>");
            message.Append("</tr>");

            message.Append("<tr>");
            message.Append($"<td>TTL Kimeneti terv</td>");
            message.Append($"<td>{_TtlPaln}</td>");
            message.Append("</tr>");

            message.Append("<tr>");
            message.Append($"<td>Sales terv</td>");
            message.Append($"<td>{_SalesPlan}</td>");
            message.Append("</tr>");

            message.Append("<tr>");
            message.Append($"<td>Sales terv DC</td>");
            message.Append($"<td>{_SalesPlanDC}</td>");
            message.Append("</tr>");

            message.Append("<tr>");
            message.Append($"<td>DC Movement</td>");
            message.Append($"<td>{_DcMovement}</td>");
            message.Append("</tr>");

            message.Append(_emailTail);

            return message.ToString();
        }

        public string BuildTestEmail()
        {
            StringBuilder message_test = new StringBuilder();
            message_test.Append(_testEmail);
            message_test.Append($"<p>This test mail has been sent to {_testEmailAddr} email address.</p>");
            message_test.Append(_testEmailTail);
            return message_test.ToString();
        }
    }
}
