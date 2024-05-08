using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ex10_MovieFinder2024
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

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            //await this.ShowMessageAsync("검색", "검색을 시작합니다!!");
            if (string.IsNullOrEmpty(TxtMovieName.Text))
            {
                await this.ShowMessageAsync("검색", "검색할 영화명을 입력하세요!!");
                return;
            }

            SearchMovie(TxtMovieName.Text);
        }

        private async void SearchMovie(string movieName)
        {
            string tmdb_apiKey = "fd289ba75f5d775304ca26173beceb55"; // TMDB사이트에서 제공받은 API키.
            string encoding_movieName = HttpUtility.UrlEncode(movieName, Encoding.UTF8);
            string openApiUri = $"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apiKey}" +
                                $"&language=ko-KR&page=1&include_adult=false&query={encoding_movieName}";
            // Debug.WriteLine(openApiUri);

            string result = string.Empty; // 결과값 

            // openapi 실행 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                // tmdb api 요청
                req = WebRequest.Create(openApiUri); // URL을 넣어서 객체를 생성
                res = await req.GetResponseAsync(); // 요청한 URL의 결과를 비동기응답
                reader = new StreamReader(res.GetResponseStream()); // 
                result = reader.ReadToEnd();  // json결과를 문자열로 저장

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
                // TODO : 메시지박스로 출력
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            // result string을 json으로 변경
            var jsonResult = JObject.Parse(result); // type.Parse(string)
            var total = Convert.ToInt32(jsonResult["total_results"]);
            //await this.ShowMessageAsync("검색수", total.ToString());
            var results = jsonResult["results"];
            var jsonArray = results as JArray; // results가 json 배열이기 때문에 JArray는 List와 동일해서 foreach 사용가능
        }

        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e)
        {
             
        }

        private async void GrdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            await this.ShowMessageAsync("포스터", "포스터처리합니다");
        }

        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("즐겨찾기", "즐겨찾기 추가합니다!");
        }

        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("즐겨찾기", "즐겨찾기 확인합니다!!");
        }

        private async void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("즐겨찾기", "즐겨찾기 삭제합니다.");
        }

        private async void BtnWatchTrailer_Click(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("유튜브예고편", "예고편 확인합니다");
        }

    }
}