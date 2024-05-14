﻿using System.Diagnostics;
using System.IO;
using System.Net;
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
using Daegu_Restaurant.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace Daegu_Restaurant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnReqRealtime_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = "https://www.daegufood.go.kr/kor/api/tasty.html?mode=json&addr=%EC%A4%91%EA%B5%AC";
            string result = string.Empty;

            // WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                //await this.ShowMessageAsync("결과", result);
                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            var status = Convert.ToString(jsonResult["status"]);

            if (status == "DONE")
            {
                var data = jsonResult["data"];
                var jsonArray = data as JArray; // json자체에서 []안에 들어간 배열데이터만 JArray 변환가능

                var restaurants = new List<Restaurant>();
                foreach (var item in jsonArray)
                {
                    restaurants.Add(new Restaurant()
                    {
                        Id = 0,
                        Cnt = Convert.ToInt32(item["cnt"]),
                        Opendata_id = Convert.ToString(item["OPENDATA_ID"]),
                        Gng_cs = Convert.ToString(item["GNG_CS"]),
                        Fd_cs = Convert.ToString(item["FD_CS"]),
                        Bz_nm = Convert.ToString(item["BZ_NM"]),
                        Tlno = Convert.ToString(item["TLNO"]),
                        Mbz_hr = Convert.ToString(item["MBZ_HR"]),
                        Seat_cnt = Convert.ToString(item["SEAT_CNT"]),
                        Pkpl = Convert.ToString(item["PKPL"]),
                        Hp = Convert.ToString(item["HP"]),
                        Psb_frn = Convert.ToString(item["PSB_FRN"]),
                        Bkn_yn = Convert.ToString(item["BKN_YN"]),
                        Infn_fcl = Convert.ToString(item["INFN_FCL"]),
                        Brft_yn = Convert.ToString(item["BRFT_YN"]),
                        Dssrt_yn = Convert.ToString(item["DSSRT_YN"]),
                        Mnu = Convert.ToString(item["MNU"]),
                        Smpl_desc = Convert.ToString(item["SMPL_DESC"]),
                        Sbw = Convert.ToString(item["SBW"]),
                        Bus = Convert.ToString(item["BUS"]),
                    });
                }

                this.DataContext = restaurants;
                StsResult.Content = $"OpenAPI {restaurants.Count}건 조회완료!";
            }
        }

        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await this.ShowMessageAsync("저장오류", "실시간 조회후 저장하십시오.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var insRes = 0;
                    foreach (Restaurant item in GrdResult.Items)
                    {
                        SqlCommand cmd = new SqlCommand(Models.Restaurant.INSERT_QUERY, conn);
                        cmd.Parameters.AddWithValue("@Cnt", item.Cnt);
                        cmd.Parameters.AddWithValue("@Opendata_id", item.Opendata_id);
                        cmd.Parameters.AddWithValue("@Gng_cs", item.Gng_cs);
                        cmd.Parameters.AddWithValue("@Fd_cs", item.Fd_cs);
                        cmd.Parameters.AddWithValue("@Bz_nm", item.Bz_nm);
                        cmd.Parameters.AddWithValue("@Tlno", item.Tlno);
                        cmd.Parameters.AddWithValue("@Mbz_hr", item.Mbz_hr);
                        cmd.Parameters.AddWithValue("@Seat_cnt", item.Seat_cnt);
                        cmd.Parameters.AddWithValue("@Pkpl", item.Pkpl);
                        cmd.Parameters.AddWithValue("@Hp", item.Hp);
                        cmd.Parameters.AddWithValue("@Psb_frn", item.Psb_frn);
                        cmd.Parameters.AddWithValue("@Bkn_yn", item.Bkn_yn);
                        cmd.Parameters.AddWithValue("@Infn_fcl", item.Infn_fcl);
                        cmd.Parameters.AddWithValue("@Brft_yn", item.Brft_yn);
                        cmd.Parameters.AddWithValue("@Dssrt_yn", item.Dssrt_yn);
                        cmd.Parameters.AddWithValue("@Mnu", item.Mnu);
                        cmd.Parameters.AddWithValue("@Smpl_desc", item.Smpl_desc);
                        cmd.Parameters.AddWithValue("@Sbw", item.Bkn_yn);
                        cmd.Parameters.AddWithValue("@Bus", item.Bus);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    if (insRes > 0)
                    {
                        await this.ShowMessageAsync("저장", "DB저장성공!");
                        StsResult.Content = $"DB저장 {insRes}건 성공!";
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("저장오류", $"저장오류 {ex.Message}");
            }
        }

        private void CboReqDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}