using System.Data;
using System.Diagnostics;
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
using CefSharp.DevTools.Debugger;
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
        private bool isFavorite = false;

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
                isFavorite = false;
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

        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GrdResult.SelectedItem is Restaurant selectedRestaurant)
            {
                // 선택된 레스토랑의 위치를 가져옵니다. 여기서는 Gng_cs 필드를 사용한다고 가정합니다.
                string location = selectedRestaurant.Gng_cs;

                // 지도 창을 엽니다.
                MapWindow mapWindow = new MapWindow(location);
                mapWindow.Show();
            }

        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();
                    string searchKeyword = TxtMenuName.Text.Trim();

                    // 초기화: 기존 결과를 초기화합니다.
                    GrdResult.ItemsSource = null;

                    string sql = "SELECT * FROM Restaurant WHERE Mnu LIKE @searchKeyword";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@searchKeyword", "%" + searchKeyword + "%");

                    SqlDataReader reader = cmd.ExecuteReader();

                    var searchResults = new List<Restaurant>();


                    while (reader.Read())
                    {
                        var restaurant = new Restaurant();

                        restaurant.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        restaurant.Cnt = reader.GetInt32(reader.GetOrdinal("Cnt"));
                        restaurant.Gng_cs = reader["Gng_cs"].ToString();
                        restaurant.Fd_cs = reader["Fd_cs"].ToString();
                        restaurant.Bz_nm = reader["Bz_nm"].ToString();
                        restaurant.Tlno = reader["Tlno"].ToString();
                        restaurant.Mbz_hr = reader["Mbz_hr"].ToString();
                        restaurant.Seat_cnt = reader["Seat_cnt"].ToString();
                        restaurant.Pkpl = reader["Pkpl"].ToString();
                        restaurant.Hp = reader["Hp"].ToString();
                        restaurant.Psb_frn = reader["Psb_frn"].ToString();
                        restaurant.Bkn_yn = reader["Bkn_yn"].ToString();
                        restaurant.Infn_fcl = reader["Infn_fcl"].ToString();
                        restaurant.Brft_yn = reader["Brft_yn"].ToString();
                        restaurant.Dssrt_yn = reader["Dssrt_yn"].ToString();
                        restaurant.Mnu = reader["Mnu"].ToString();
                        restaurant.Smpl_desc = reader["Smpl_desc"].ToString();
                        restaurant.Sbw = reader["Sbw"].ToString();
                        restaurant.Bus = reader["Bus"].ToString();

                        searchResults.Add(restaurant);
                    }
                    reader.Close();

                    GrdResult.ItemsSource = searchResults;

                    StsResult.Content = $"검색 완료: {searchResults.Count}건";
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("검색 오류", $"검색 중 오류가 발생했습니다: {ex.Message}");
            }


        }

        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("즐겨찾기", "추가할 맛집을 선택하세요(복수선택가능).");
                return;
            }

            if (isFavorite == true) // 즐겨찾기 보기한 뒤 영화를 다시 즐겨찾기하려고할때 막음.
            {
                await this.ShowMessageAsync("즐겨찾기", "이미 즐겨찾기한 맛집입니다.");
                return;
            }

            var addMovieItems = new List<Favorite>();
            foreach (Restaurant item in GrdResult.SelectedItems)
            {
                addMovieItems.Add(new Favorite()
                {
                    Id = item.Id,
                    Cnt = item.Cnt,
                    Opendata_id = item.Opendata_id,
                    Gng_cs = item.Gng_cs,
                    Fd_cs = item.Fd_cs,
                    Bz_nm = item.Bz_nm,
                    Tlno = item.Tlno,
                    Mbz_hr = item.Mbz_hr,
                    Seat_cnt = item.Seat_cnt,
                    Pkpl = item.Pkpl,
                    Hp = item.Hp,
                    Psb_frn = item.Psb_frn,
                    Bkn_yn = item.Bkn_yn,
                    Infn_fcl = item.Infn_fcl,
                    Brft_yn = item.Brft_yn,
                    Dssrt_yn = item.Dssrt_yn,
                    Mnu = item.Mnu,
                    Smpl_desc = item.Smpl_desc,
                    Sbw = item.Sbw,
                    Bus = item.Bus,
                });
            }

            Debug.WriteLine(addMovieItems.Count);
            try
            {
                var insRes = 0;

                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    foreach (Favorite item in addMovieItems)
                    {
                        // 저장되기 전에 이미 저장된 데이터인지 확인 후 
                        SqlCommand chkCmd = new SqlCommand(Favorite.CHECK_QUERY, conn);
                        chkCmd.Parameters.AddWithValue("@Cnt", item.Cnt);
                        var cnt = Convert.ToInt32(chkCmd.ExecuteScalar()); // COUNT(*) 등의 1row, 1coloumn값을 리턴할때

                        if (cnt == 1) continue;  // 이미 데이터가 있으면 패스

                        SqlCommand cmd = new SqlCommand(Models.Favorite.INSERT_QUERY, conn);
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
                        cmd.Parameters.AddWithValue("@Sbw", item.Sbw);
                        cmd.Parameters.AddWithValue("@Bus", item.Bus);

                        insRes += cmd.ExecuteNonQuery(); // 데이터 하나마다 INSERT쿼리 실행
                    }
                }

                if (insRes == addMovieItems.Count)
                {
                    await this.ShowMessageAsync("즐겨찾기", $"즐겨찾기 {insRes}건 저장성공!");
                }
                else
                {
                    await this.ShowMessageAsync("즐겨찾기", $"즐겨찾기 {addMovieItems.Count}건중 {insRes}건 저장성공!");
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 오류 {ex.Message}");
            }
            BtnViewFavorite_Click(sender, e); // 저장후 저장된 즐겨찾기 바로보기
        }

        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;  // 데이터그리드에 보낸 데이터를 모두 삭제
            TxtMenuName.Text = string.Empty;

            List<Favorite> favorites = new List<Favorite>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var cmd = new SqlCommand(Models.Favorite.SELECT_QUERY, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "Favorite");

                    foreach (DataRow row in dSet.Tables["Favorite"].Rows)
                    {
                        var favorite = new Favorite()
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Cnt = Convert.ToInt32(row["Cnt"]),
                            Opendata_id = Convert.ToString(row["Opendata_id"]),
                            Gng_cs = Convert.ToString(row["Gng_cs"]),
                            Fd_cs = Convert.ToString(row["Fd_cs"]),
                            Bz_nm = Convert.ToString(row["Bz_nm"]),
                            Tlno = Convert.ToString(row["Tlno"]),
                            Mbz_hr = Convert.ToString(row["Mbz_hr"]),
                            Seat_cnt = Convert.ToString(row["Seat_cnt"]),
                            Pkpl = Convert.ToString(row["Pkpl"]),
                            Hp = Convert.ToString(row["Hp"]),
                            Psb_frn = Convert.ToString(row["Psb_frn"]),
                            Bkn_yn = Convert.ToString(row["Bkn_yn"]),
                            Infn_fcl = Convert.ToString(row["Infn_fcl"]),
                            Brft_yn = Convert.ToString(row["Brft_yn"]),
                            Dssrt_yn = Convert.ToString(row["Dssrt_yn"]),
                            Mnu = Convert.ToString(row["Mnu"]),
                            Smpl_desc = Convert.ToString(row["Smpl_desc"]),
                            Sbw = Convert.ToString(row["Sbw"]),
                            Bus = Convert.ToString(row["Bus"]),
                        };

                        favorites.Add(favorite);
                    }

                    this.DataContext = favorites;
                    isFavorite = true; // 즐겨찾기 DB에서 
                    StsResult.Content = $"즐겨찾기 {favorites.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 조회오류 {ex.Message}");
            }
        }

        private async void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (isFavorite == false)
            {
                await this.ShowMessageAsync("삭제", "즐겨찾기한 맛집이 아닙니다.");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("삭제", "삭제할 맛집을 선택하세요.");
                return;
            }

            // 삭제시작!
            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var delRes = 0;

                    foreach (Favorite item in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(Models.Favorite.DELETE_QUERY, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await this.ShowMessageAsync("삭제", $"즐겨찾기 {delRes}건 삭제");
                    }
                    else
                    {
                        await this.ShowMessageAsync("삭제", $"즐겨찾기 {GrdResult.SelectedItems.Count}건중 {delRes} 건 삭제");
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 삭제 오류 {ex.Message}");
            }

            BtnViewFavorite_Click(sender, e); // 즐겨찾기 보기 재실행!
        }
    }
}