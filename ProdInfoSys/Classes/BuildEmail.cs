using ProdInfoSys.Models.NonRelationalModels;
using System.Text;

namespace ProdInfoSys.Classes
{
    /// <summary>
    /// Provides methods for generating formatted HTML email messages for meeting minutes, leader summaries, and test
    /// notifications.
    /// </summary>
    /// <remarks>The BuildEmail class supports creating different types of email content, including tables of
    /// open tasks and summary reports, by accepting relevant data through its constructors. Use the appropriate
    /// constructor based on the type of email to be generated. The generated HTML is intended for use in email clients
    /// that support HTML formatting.</remarks>
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

        /// <summary>
        /// Initializes a new instance of the BuildEmail class using the specified collection of meeting minutes.
        /// </summary>
        /// <param name="meetingMinutes">A list of MeetingMinutes objects to include in the email build process. Cannot be null.</param>
        public BuildEmail(List<MeetingMinutes> meetingMinutes)
        {
            _memos = meetingMinutes;
        }

        /// <summary>
        /// Initializes a new instance of the BuildEmail class with the specified test email address.
        /// </summary>
        /// <param name="testEmailAddr">The email address to use for testing purposes. Cannot be null or empty.</param>
        public BuildEmail(string testEmailAddr)
        {
            _testEmailAddr = testEmailAddr;
        }

        /// <summary>
        /// Initializes a new instance of the BuildEmail class with the specified pricing, cost, and sales plan details.
        /// </summary>
        /// <param name="SamplePrice">The sample price value to include in the email content.</param>
        /// <param name="PlanPrice">The plan price value to include in the email content.</param>
        /// <param name="PlanDCPrice">The plan distribution center price value to include in the email content.</param>
        /// <param name="MaterialCost">The material cost value to include in the email content.</param>
        /// <param name="MaterialCostRepack">The material cost for repackaging to include in the email content.</param>
        /// <param name="MaterialCostKft">The material cost per thousand feet to include in the email content.</param>
        /// <param name="TtlPaln">The total plan value to include in the email content.</param>
        /// <param name="SalesPlan">The sales plan value to include in the email content.</param>
        /// <param name="SalesPlanDC">The sales plan distribution center value to include in the email content.</param>
        /// <param name="DcMovement">The distribution center movement value to include in the email content.</param>
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

        /// <summary>
        /// Builds and returns an HTML-formatted email message containing a table of memo items.
        /// </summary>
        /// <remarks>The returned HTML message includes a table where each row represents a memo item with
        /// its associated details. Ensure that the resulting HTML is properly embedded in an email body to display the
        /// table correctly in email clients.</remarks>
        /// <returns>A string containing the complete HTML email message with all memo items formatted as table rows.</returns>
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

        /// <summary>
        /// Builds the HTML-formatted email message containing pricing and cost details for the leader report.
        /// </summary>
        /// <remarks>The returned HTML message includes several rows with labels and corresponding values
        /// for sample price, plan prices, material costs, sales plans, and DC movement. The structure and content of
        /// the message depend on the values of the underlying fields at the time of the method call.</remarks>
        /// <returns>A string containing the complete HTML email message with pricing and cost information formatted as a table.</returns>
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

        /// <summary>
        /// Builds and returns the HTML content of a test email message for the configured test recipient.
        /// </summary>
        /// <returns>A string containing the HTML body of the test email message.</returns>
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
