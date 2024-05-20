using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daegu_Restaurant.Models
{
    public class Favorite
    {
        public int Id { get; set; } // 추가생성, DB에 넣을때 사용할 값
        public int Cnt { get; set; } // 순번
        public string Opendata_id { get; set; } // 고유 번호
        public string Gng_cs { get; set; } // 주소
        public string Fd_cs { get; set; } // 음식카테고리
        public string Bz_nm { get; set; } // 음식점명
        public string Tlno { get; set; } // 연락처
        public string Mbz_hr { get; set; } // 영업시간
        public string Seat_cnt { get; set; } // 좌석수
        public string Pkpl { get; set; } // 주차장
        public string Hp { get; set; } // 웹사이트
        public string Psb_frn { get; set; } // 가능언어
        public string Bkn_yn { get; set; } // 예약가능여부
        public string Infn_fcl { get; set; } // 놀이시설여부
        public string Brft_yn { get; set; } // 조식여부
        public string Dssrt_yn { get; set; } // 후식여부
        public string Mnu { get; set; } // 메뉴
        public string Smpl_desc { get; set; } // 설명
        public string Sbw { get; set; } // 지하철 오시는길
        public string Bus { get; set; } // 버스 오시는길

        public static readonly string CHECK_QUERY = @"SELECT COUNT(*)
                                                        FROM Favorite
                                                       WHERE Cnt = @Cnt";

        public static readonly string SELECT_QUERY = @"SELECT [Id]
                                                            ,[Cnt]
                                                            ,[Opendata_id]
                                                            ,[Gng_cs]
                                                            ,[Fd_cs]
                                                            ,[Bz_nm]
                                                            ,[Tlno]
                                                            ,[Mbz_hr]
                                                            ,[Seat_cnt]
                                                            ,[Pkpl]
                                                            ,[Hp]
                                                            ,[Psb_frn]
                                                            ,[Bkn_yn]
                                                            ,[Infn_fcl]
                                                            ,[Brft_yn]
                                                            ,[Dssrt_yn]
                                                            ,[Mnu]
                                                            ,[Smpl_desc]
                                                            ,[Sbw]
                                                            ,[Bus]
                                                        FROM [dbo].[Favorite]";

        public static readonly string INSERT_QUERY = @"INSERT INTO [dbo].[Favorite]
                                                                        ([Cnt]
                                                                        ,[Opendata_id]
                                                                        ,[Gng_cs]
                                                                        ,[Fd_cs]
                                                                        ,[Bz_nm]
                                                                        ,[Tlno]
                                                                        ,[Mbz_hr]
                                                                        ,[Seat_cnt]
                                                                        ,[Pkpl]
                                                                        ,[Hp]
                                                                        ,[Psb_frn]
                                                                        ,[Bkn_yn]
                                                                        ,[Infn_fcl]
                                                                        ,[Brft_yn]
                                                                        ,[Dssrt_yn]
                                                                        ,[Mnu]
                                                                        ,[Smpl_desc]
                                                                        ,[Sbw]
                                                                        ,[Bus])
                                                                    VALUES
                                                                        (@Cnt
                                                                        ,@Opendata_id
                                                                        ,@Gng_cs
                                                                        ,@Fd_cs
                                                                        ,@Bz_nm
                                                                        ,@Tlno
                                                                        ,@Mbz_hr
                                                                        ,@Seat_cnt
                                                                        ,@Pkpl
                                                                        ,@Hp
                                                                        ,@Psb_frn
                                                                        ,@Bkn_yn
                                                                        ,@Infn_fcl
                                                                        ,@Brft_yn
                                                                        ,@Dssrt_yn
                                                                        ,@Mnu
                                                                        ,@Smpl_desc
                                                                        ,@Sbw
                                                                        ,@Bus)";

        public static readonly string DELETE_QUERY = @"DELETE FROM [dbo].[Favorite] WHERE Id = @id";
    }
}
