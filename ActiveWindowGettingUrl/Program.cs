#region Imports

using System.Runtime.InteropServices; //required
using UIAutomationClient; //required   
using TreeScope = UIAutomationClient.TreeScope; // required
using System.Threading;
using System.Diagnostics;

#endregion


namespace ActiveWindowGettingUrl
{
    class Program
	{

        #region Threading timer

        static Timer? TTimer;
        static void TickTimer(object? state = null)
        {
            Console.WriteLine(url);
            GC.Collect();
        }

        #endregion


        private CUIAutomation _automation;
        static string url = "null 1";

        public Program()
        {
            _automation = new CUIAutomation();
            _automation.AddFocusChangedEventHandler(null, new FocusChangeHandler(this));
            Console.Title = "Active Browser Window Getting Url Address";
        }


        static void Main(string[] args)
        {
            Program a = new Program();
            a.MemberwiseClone();

            TTimer = new Timer(
            new TimerCallback(TickTimer),
            null,
            1000,
            1000);

            Console.Read();

        }


        public class FocusChangeHandler : IUIAutomationFocusChangedEventHandler
        {

            private readonly Program _listener;


            public FocusChangeHandler(Program listener)
            {
                _listener = listener;
            }


            public void HandleFocusChangedEvent(IUIAutomationElement element)
            {
                if (element != null)
                {
                    using (Process process = Process.GetProcessById(element.CurrentProcessId))
                    {
                        try
                        {
                            IUIAutomationElement elm = this._listener._automation.ElementFromHandle(process.MainWindowHandle);
                            IUIAutomationCondition Cond = this._listener._automation.CreatePropertyCondition(30003, 50004);
                            IUIAutomationElementArray elm2 = elm.FindAll(TreeScope.TreeScope_Descendants, Cond);

                            for (int i = 0; i < elm2.Length; i++)
                            {
                                IUIAutomationElement elm3 = elm2.GetElement(i);
                                IUIAutomationValuePattern val = (IUIAutomationValuePattern)elm3.GetCurrentPattern(10002);

                                if (val.CurrentValue != "")
                                {
                                    bool isUri = Uri.IsWellFormedUriString(val.CurrentValue, UriKind.RelativeOrAbsolute);

                                    if (isUri)
                                    {
                                        if (!val.CurrentValue.Contains("http"))
                                        {
                                            url = "http://" + val.CurrentValue;
                                        }

                                        else url = val.CurrentValue;

                                        break;
                                    }

                                    else
                                        url = "null";

                                }

                                else url = "null";
                            }
                        }

                        catch { url = "Url isn't found"; }

                    }
                }

            }
        }

    }
}