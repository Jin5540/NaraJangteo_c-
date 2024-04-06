using System;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;

namespace NaraJanteo_c_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void crawlButton_Click(object sender, EventArgs e)
        {
            // 사용자가 선택한 시작 및 종료 날짜 가져오기
            DateTime startDate = startDatePicker.Value;
            DateTime endDate = endDatePicker.Value;

            // startDate가 endDate보다 클 경우 경고창을 표시
            if (startDate > endDate)
            {
                MessageBox.Show("시작 날짜가 종료 날짜보다 큽니다. 다시 선택해주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 엑셀 파일 경로 선택
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = false;
            saveFileDialog.Filter = "Excel 파일 (*.xlsx)|*.xlsx";
            saveFileDialog.Title = "엑셀 파일 저장 위치 선택";
            saveFileDialog.ShowDialog();
            string filePath = saveFileDialog.FileName;

            // 크롤링 및 데이터 저장
            CrawlAndSaveData(startDate, endDate, filePath);

            MessageBox.Show("데이터 저장이 완료되었습니다.");
        }

        private void CrawlAndSaveData(DateTime startDate, DateTime endDate, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (IWebDriver driver = new ChromeDriver())
            {
                // 크롤링할 웹사이트의 URL 설정
                string url = "https://www.g2b.go.kr/index.jsp";

                // 웹페이지 열기
                driver.Navigate().GoToUrl(url);

                // 검색창 요소를 찾아서 "rpa"를 입력
                IWebElement searchBox = driver.FindElement(By.XPath("//*[@id=\'bidNm\']"));
                searchBox.SendKeys("rpa");

                var setFromDays = driver.FindElement(By.XPath("//*[@id=\'noticedate\']/button[1]"));
                setFromDays.Click();

                var fiveY = startDate.Year.ToString();
                var fiveM = string.Format("{0:MM}", startDate);
                var IfiveM = int.Parse(fiveM);
                var fiveD = startDate.Day.ToString();
                var IfiveY = int.Parse(fiveY);

                var aMonth = driver.FindElement(By.XPath("//*[@id=\"ui-datepicker-div\"]/div[1]/div/span[2]")).Text;
                var aYear = driver.FindElement(By.XPath("//*[@id=\'ui-datepicker-div\']/div[1]/div/span[1]")).Text;

                int iMonth = 0;
                int iYear = 0;

                for (int i = 0; i < aMonth.Length; i++)
                {
                    iMonth = iMonth * 10 + (aMonth[i] - '0');
                }

                for (int i = 0; i < aYear.Length; i++)
                {
                    iYear = iYear * 10 + (aYear[i] - '0');
                }

                if (iMonth > IfiveM) {
                    while (!(aMonth.Equals(fiveM) && aYear.Equals(fiveY)))
                    {
                        driver.FindElement(By.XPath("//*[@id=\'datepicker-prev\']")).Click();
                        aMonth = driver.FindElement(By.XPath("//*[@id=\"ui-datepicker-div\"]/div[1]/div/span[2]")).Text;
                        aYear = driver.FindElement(By.XPath("//*[@id=\'ui-datepicker-div\']/div[1]/div/span[1]")).Text;
                    }
                }if (iMonth < IfiveM){
                    while (!(aMonth.Equals(fiveM) && aYear.Equals(fiveY)))
                    {
                        driver.FindElement(By.ClassName("ui-datepicker-next")).Click();
                        aMonth = driver.FindElement(By.XPath("//*[@id=\"ui-datepicker-div\"]/div[1]/div/span[2]")).Text;
                        aYear = driver.FindElement(By.XPath("//*[@id=\'ui-datepicker-div\']/div[1]/div/span[1]")).Text;
                    }
                }

                Debug.WriteLine("시작 날짜 ::: " + fiveY + "/" + fiveM + "/" + fiveD);

                IWebElement calandar = driver.FindElement(By.XPath("//*[@id=\'ui-datepicker-div\']/table"));
                IWebElement tbody = calandar.FindElement(By.TagName("tbody"));
                var trs = tbody.FindElements(By.TagName("tr"));

                foreach (var tr in trs)
                  {
                     var tds = tr.FindElements(By.TagName("td"));
                     foreach (var td in tds)
                     {
                        if (td.Text.Equals(fiveD))
                         {
                            td.Click();
                         }
                      }
                }
                
                var setToDays = driver.FindElement(By.XPath("//*[@id=\'noticedate\']/button[2]"));
                setToDays.Click();

                var NowY = endDate.Year.ToString();
                var NowM = string.Format("{0:MM}", endDate);
                var InowM = int.Parse(NowM);
                var NowD = endDate.Day.ToString();
                var InowY = int.Parse(NowY);

                var eMonth = driver.FindElement(By.XPath("//*[@id=\'ui-datepicker-div\']/div[1]/div/span[2]")).Text;
                var eYear = driver.FindElement(By.XPath("//*[@id=\'ui-datepicker-div\']/div[1]/div/span[1]")).Text;

                int ieMonth = 0;
                int ieYear = 0;

                for (int i = 0; i < eMonth.Length; i++)
                {
                    ieMonth = ieMonth * 10 + (eMonth[i] - '0');
                }

                for (int i = 0; i < eYear.Length; i++)
                {
                    ieYear = ieYear * 10 + (eYear[i] - '0');
                }

                if (iMonth > IfiveM)
                {
                    while (!(eMonth.Equals(NowM) && eYear.Equals(NowY)))
                    {
                        driver.FindElement(By.XPath("//*[@id=\'datepicker-prev\']")).Click();
                        aMonth = driver.FindElement(By.XPath("//*[@id=\'ui-datepicker-div\']/div[1]/div/span[2]")).Text;
                        aYear = driver.FindElement(By.XPath("//*[@id=\'ui-datepicker-div\']/div[1]/div/span[1]")).Text;
                    }
                }
                if (iMonth < IfiveM)
                {
                    while (!(eMonth.Equals(NowM) && eYear.Equals(NowY)))
                    {
                        driver.FindElement(By.ClassName("ui-datepicker-next")).Click();
                        aMonth = driver.FindElement(By.XPath("//*[@id=\"ui-datepicker-div\"]/div[1]/div/span[2]")).Text;
                        aYear = driver.FindElement(By.XPath("//*[@id=\'ui-datepicker-div\']/div[1]/div/span[1]")).Text;
                    }
                }

                Debug.WriteLine("끝나는 날짜 ::: " + NowY + "/" + NowM + "/" + NowD);

                IWebElement Ncalandar = driver.FindElement(By.XPath("//*[@id=\'ui-datepicker-div\']/table"));
                IWebElement Ntbody = Ncalandar.FindElement(By.TagName("tbody"));
                var Ntrs = Ntbody.FindElements(By.TagName("tr"));

                foreach (var tr in Ntrs)
                {
                    var tds = tr.FindElements(By.TagName("td"));
                    foreach (var td in tds)
                    {
                        if (td.Text.Equals(NowD))
                        {
                            td.Click();
                        }
                    }
                }

                IWebElement searchButton = driver.FindElement(By.XPath("//*[@id=\'searchForm\']/div/fieldset[1]/ul/li[4]/dl/dd[3]/a"));

                // 검색 버튼 클릭
                searchButton.Click();

                Debug.WriteLine("____________________검색_____________________");

                // 페이지가 로드될 때까지 대기 (예: 10초)
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);

                try
                {
                    IWebElement firstFrame = driver.FindElement(By.XPath("//*[@id=\'sub\']"));
                    driver.SwitchTo().Frame(firstFrame); // 첫 번째 frame으로 전환
                    driver.SwitchTo().Frame("main"); // 두 번째 frame으로 전환

                    IWebElement table = driver.FindElement(By.XPath("//*[@id=\'resultForm\']/div[2]/table"));

                    IWebElement Rtbody = table.FindElement(By.TagName("tbody"));
                    var Rtrs = Rtbody.FindElements(By.TagName("tr"));

                    string[] data = { "공고명", "검색 키워드", "공고기관", "수요기관", "입력일시", "검색일시" };

                    bool fileExists = System.IO.File.Exists(filePath);
                    FileInfo fi = new FileInfo(filePath);

 
                    using (ExcelPackage pck = new ExcelPackage(fi))
                    {
                        ExcelWorksheet wsheet;

                        if (fileExists)
                        {
                            wsheet = pck.Workbook.Worksheets["Sheet1"];
                        }
                        else {
                            wsheet = pck.Workbook.Worksheets.Add("Sheet1");

                            for (int k = 0; k < data.Length; k++)
                            {
                                wsheet.Cells[1, k + 1].Value = data[k];
                            }
                        }
                        int lastRow = wsheet.Dimension.End.Row + 1;

                        for (int j = 0; j < Rtrs.Count; j++){
                           var tds = Rtrs[j].FindElements(By.TagName("td"));
                           for (int i = 3; i < tds.Count - 2; i++)
                           {
                              var div = tds[i].FindElement(By.TagName("div"));
                              wsheet.Cells[lastRow, i - 2].Value = div.Text;
                           }
                           wsheet.Cells[lastRow, data.Length].Value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                         }
                         pck.Save();

                            Debug.WriteLine("엑셀 저장 성공");
                        }


                    }
                catch (NoSuchElementException e)
                {
                    Debug.WriteLine(e.ToString() + "::: error!_____________________");
                }
                catch (NoSuchFrameException ne)
                {
                    Debug.WriteLine(ne.ToString() + "::: error!_____________________");
                }

                // WebDriver 종료
                driver.Quit();
            }
        
    }
    }
}
