using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB2SQlClass.DB2
{
  public  class GetSqlQuery
    {
        public static string GetEMployDetails(string SearchMember, string SortingColumn, string Orderby, int page, int size)
        {

            string Sqlquery = "";
            Sqlquery = @"SELECT EMSSN as SSN,EMNAME as Member,EMMEM# as ID,EMCITY as City,EMST as State,EMDOBY as Year,EMDOBM as Month,EMDOBD as Day
                    FROM EMPYP where EMDROP<> 'D'";

            if (SearchMember != "")
            {
                if ((SearchMember).All(char.IsNumber))
                {
                    Sqlquery += @" and (EMSSN =" + SearchMember + " OR EMMEM# LIKE '%" + SearchMember + "%')";
                }


                else
                {
                    string[] splitstring = SearchMember.Split(',', ' ');

                    foreach (var item in splitstring)
                    {
                        if (item != "")
                        {
                            Sqlquery += @"and (LOWER(EMNAME) LIKE LOWER('%" + item + "%'))";
                        }
                    }


                }
            }
            if (SortingColumn != "" && Orderby != "")
            {
                if (SortingColumn != "DOB")
                {
                    Sqlquery += @"ORDER BY " + SortingColumn + " " + Orderby + " ";
                }
                else
                {
                    Sqlquery += @"ORDER BY EMDOBY " + Orderby + ", EMDOBM " + Orderby + ", EMDOBD " + Orderby + " ";
                }

            }
            Sqlquery += @" OFFSET (" + page + " -1) * " + size + " ROWS FETCH NEXT " + size + " ROWS ONLY";
            return Sqlquery;
        }

        public static string TotalMemeberCount(string SearchMember)
        {

            string Sqlquery = "";
            Sqlquery = @"SELECT Count(*) AS Total
                                     FROM EMPYP where EMDROP<> 'D'";

            if (SearchMember != "")
            {
                if ((SearchMember).All(char.IsNumber))
                {
                    Sqlquery += @" and (EMSSN =" + SearchMember + " OR EMMEM# LIKE '%" + SearchMember + "%')";
                }


                else
                {
                    string[] splitstring = SearchMember.Split(',', ' ');

                    foreach (var item in splitstring)
                    {
                        if (item != "")
                        {
                            Sqlquery += @"and (LOWER(EMNAME) LIKE LOWER('%" + item + "%'))";
                        }
                    }

                }
            }
            return Sqlquery;

        }
        public static string GetEMployDetails_GlobalSearch(string SearchMember, string SortingColumn, string Orderby, long UserId, int page, int size)
        {
            string Sqlquery = "";
            Sqlquery = @"SELECT FUNDDS as Client,EMPSSN as SSN,fullname as Member,ALTID as ID,CITY as City,[State] as State,datepart(year, MBRDOB) as Year,datepart(month, MBRDOB) as Month,datepart(day, MBRDOB) as Day
                                            FROM [BICC_REPORTING].dbo.EMPYP EMP
											inner join [dbo].[User_Funds] UF on EMP.client=UF.Fund  
                                            inner join [BICC_REPORTING].dbo.CLIENTS_ABC CA on CA.CLIENT=UF.Fund 
                                            WHERE UF.UserId=" + UserId + " and  EMPYP_DROP <> 'D'";


            if (SearchMember != "")
            {
                if ((SearchMember).All(char.IsNumber))
                {

                    Sqlquery += @"and (EMPSSN ='" + SearchMember + "' OR ALTID ='" + SearchMember + "')";

                }
                else
                {
                    string[] splitstring = SearchMember.Split(',', ' ');

                    foreach (var item in splitstring)
                    {
                        if (item != "")
                        {
                            Sqlquery += @"and (LOWER(Fullname) LIKE LOWER('%" + item + "%'))";
                        }
                    }


                }
            }
            if (SortingColumn != "" && Orderby != "")
            {
                if (SortingColumn == "EMSSN")
                {
                    Sqlquery += @"ORDER BY  EMPSSN " + Orderby + " ";
                }
                else if (SortingColumn == "Client")
                {
                    Sqlquery += @"ORDER BY  FUNDDS " + Orderby + " ";
                }
                else if (SortingColumn == "EMNAME")
                {
                    Sqlquery += @"ORDER BY  fullname " + Orderby + " ";
                }
                else if (SortingColumn == "EMMEM#")
                {
                    Sqlquery += @"ORDER BY  ALTID " + Orderby + " ";
                }
                else if (SortingColumn == "EMCITY")
                {
                    Sqlquery += @"ORDER BY  CITY " + Orderby + " ";
                }
                else if (SortingColumn == "EMST")
                {
                    Sqlquery += @"ORDER BY  State " + Orderby + " ";
                }
                else if (SortingColumn == "DOB")
                {
                    Sqlquery += @"ORDER BY  MBRDOB " + Orderby + " ";
                }
                else
                {
                    Sqlquery += @"ORDER BY " + SortingColumn + " " + Orderby + " ";
                }

            }
            Sqlquery += @" OFFSET " + size + " * (" + page + " - 1) ROWS  FETCH NEXT " + size + " ROWS ONLY";

            return Sqlquery;
        }
        public static string GlobalSearchTotalCount(string SearchMember, long UserId)
        {

            string Sqlquery = "";
            Sqlquery = @"SELECT count(*) As TotalCount
                                            FROM [BICC_REPORTING].dbo.EMPYP EMP
											inner join [dbo].[User_Funds] UF on EMP.client=UF.Fund  
                                             inner join [BICC_REPORTING].dbo.CLIENTS_ABC CA on CA.CLIENT=UF.Fund 
                                            WHERE UF.UserId=" + UserId + " and  EMPYP_DROP <> 'D'";

            if (SearchMember == "")
            {

                return Sqlquery;
            }
            else
            {
                if ((SearchMember).All(char.IsNumber))
                {

                    Sqlquery += @"and (EMPSSN ='" + SearchMember + "' OR ALTID ='" + SearchMember + "')";
                    return Sqlquery;
                }
                else
                {
                    string[] splitstring = SearchMember.Split(',', ' ');

                    foreach (var item in splitstring)
                    {
                        if (item != "")
                        {
                            Sqlquery += @"and (LOWER(Fullname) LIKE LOWER('%" + item + "%'))";
                        }
                    }

                    return Sqlquery;

                }
            }


        }

        public static string GetDependentDetails(string SSN)
        {

            return @"SELECT DPSSN AS DPSSN,DPSEQ AS SEQ, DPNAME AS NAME,CASE  WHEN DPRLTN = 1 THEN 'SPOUSE'
 WHEN DPRLTN = 2 THEN 'SON'
 WHEN DPRLTN = 3 THEN 'DAUGHTER'
 WHEN DPRLTN = 4 THEN 'STEPCHILD'
 WHEN DPRLTN = 9 THEN 'OTHER' END AS Relation,DPSTAT AS STATUS,DPDOBY AS DOBY,DPDOBM AS DOBM ,DPDOBD AS DOBD,
                    d.DPCLAS CLASS, d.DPPLAN AS  PLAN FROM DEPNP d 
                        WHERE DPSSN = '" + SSN + "'   AND DPDROP<> 'D'";
        }

        public static string GetDependentDetailsWithSeq(string SSN, int DPSEQ)
        {

            return @"SELECT WBADR1 AS ADDRESS1,WBADR2 AS ADDRESS2,WBADR3 AS ADDRESS3,WBCITY AS CITY,WBST AS STATE,WBZIP5 AS ZIP5,WBZIP4 AS ZIP4,DPSSN AS DPSSN,DPSEQ AS SEQ, DPNAME AS NAME,d.DPRLTN AS RELATION,DPSTAT AS STATUS,DPDOBY AS DOBY,DPDOBM AS DOBM ,DPDOBD AS DOBD,
                            DPEFDY AS EFDY,DPEFDM AS EFDM ,DPEFDD AS EFDD, 
                            DPTDTY AS TDTY,DPTDTM AS TDTM ,DPTDTD AS TDTD, 
                            d.DPCLAS CLASS, d.DPPLAN AS  PLAN FROM DEPNP d LEFT JOIN
                            WBENP BENP ON BENP.WBSSN  = d.DPSSN AND 
                            BENP.WBSEQ# = d.DPSEQ  
                            WHERE DPSSN = '" + SSN + "' AND DPSEQ =" + DPSEQ + "  AND DPDROP<> 'D'";

        }



        public static string GeTMemberClaims(string SSN, string ClaimNumber, DateTime? Fromdate, DateTime? Todate, string Dependent, string SortingColumn, string Orderby, int page, int size)
        {

            var FromDateQuery = "";
            if (Fromdate != null)
            {
                FromDateQuery = @" And (CHPRDY>" + Fromdate.Value.Year + " or  (CHPRDY>=" + Fromdate.Value.Year + " AND CHPRDM>" + Fromdate.Value.Month + ") or (CHPRDY>=" + Fromdate.Value.Year + " AND CHPRDM>=" + Fromdate.Value.Month + " AND CHPRDD>=" + Fromdate.Value.Day + "))";
            }
            var ToDateQuery = "";
            if (Todate != null)
            {
                ToDateQuery = @"  And (CHPRDY<" + Todate.Value.Year + " OR  (CHPRDY<=" + Todate.Value.Year + " AND CHPRDM<" + Todate.Value.Month + ") OR (CHPRDY<=" + Todate.Value.Year + " AND CHPRDM<=" + Todate.Value.Month + " AND CHPRDD<=" + Todate.Value.Day + ")) ";

            }
            var SqlQUery = @"SELECT C.CHEOB# AS EOBNo,CHCLM# as ClaimNumber,P.PRPNAM AS PROVIDER, C.CHDEP#,CASE WHEN C.CHDEP# = 0 THEN REPLACE(EMNAME,'*',',')   ELSE REPLACE(d.DPNAME,'*',',')  END AS ForPerson,
                      C.CHCLTP AS ClaimType,C.CHCLM$ AS ClaimAmount,C.CHPAY$  AS Paid,CHCOPA + CHDED$ + CHCO$  MemberPaid,CHPRDY as ClaimYear,CHPRDM ClaimMonth, CHPRDD ClaimDate
                        FROM CLMHP C INNER JOIN EMPYP e ON e.EMSSN = C.CHSSN LEFT JOIN DEPNP d  ON d.DPSSN = C.CHSSN  AND C.CHDEP# = d.DPSEQ
                         INNER JOIN AMBENDF.PROVP P ON P.PRNUM = C.CHPROV  AND P.PRSEQ# = C.CHSEQ#
                           WHERE CHSSN = '" + SSN + "'  AND CHCLM# like '%" + ClaimNumber + "%' ";

            if (Dependent != "")
            {
                SqlQUery += @" and C.CHDEP#='" + Dependent + "' ";
            }


            if (FromDateQuery != "")
            {
                SqlQUery += FromDateQuery;
            }
            if (ToDateQuery != "")
            {
                SqlQUery += ToDateQuery;
            }

            if (SortingColumn != "" && Orderby != "")
            {
                if (SortingColumn == "ClaimDate")
                {
                    SqlQUery += @" ORDER BY CHPRDY " + Orderby + ", CHPRDM " + Orderby + ", CHPRDD " + Orderby + " ";

                }
                else if (SortingColumn == "MemberPaid")
                {
                    SqlQUery += @" ORDER BY CHCOPA " + Orderby + ", CHDED$ " + Orderby + ", CHCO$ " + Orderby + " ";
                }
                else
                {
                    SqlQUery += @" ORDER BY " + SortingColumn + " " + Orderby + " ";
                }

            }
            if (page != 0 && size != 0)
            {
                SqlQUery += @" OFFSET (" + page + " -1) * " + size + " ROWS FETCH NEXT " + size + " ROWS ONLY";
            }



            return SqlQUery;
        }
        public static string GetTotalCountClaimDetailTable(string SSN, string Seq, string ClaimNumber, DateTime? Fromdate, DateTime? Todate, string Dependent)
        {
            var SqlQUery = "";

            var FromDateQuery = "";
            if (Fromdate != null)
            {
                FromDateQuery = @" And (CHPRDY>" + Fromdate.Value.Year + " or  (CHPRDY>=" + Fromdate.Value.Year + " AND CHPRDM>" + Fromdate.Value.Month + ") or (CHPRDY>=" + Fromdate.Value.Year + " AND CHPRDM>=" + Fromdate.Value.Month + " AND CHPRDD>=" + Fromdate.Value.Day + "))";
            }
            var ToDateQuery = "";
            if (Todate != null)
            {
                ToDateQuery = @"  And (CHPRDY<" + Todate.Value.Year + " OR  (CHPRDY<=" + Todate.Value.Year + " AND CHPRDM<" + Todate.Value.Month + ") OR (CHPRDY<=" + Todate.Value.Year + " AND CHPRDM<=" + Todate.Value.Month + " AND CHPRDD<=" + Todate.Value.Day + ")) ";

            }
            SqlQUery = @"SELECT Count(*) AS TotalCount
                        FROM CLMHP C INNER JOIN EMPYP e ON e.EMSSN = C.CHSSN LEFT JOIN DEPNP d  ON d.DPSSN = C.CHSSN  AND C.CHDEP# = d.DPSEQ
                         INNER JOIN AMBENDF.PROVP P ON P.PRNUM = C.CHPROV  AND P.PRSEQ# = C.CHSEQ#
                           WHERE CHSSN = '" + SSN + "'  AND CHCLM# like '%" + ClaimNumber + "%' ";
            if (Seq != "0")
            {
                SqlQUery += @" And  C.CHDEP# =" + Seq + " ";
            }
            if (Dependent != "")
            {
                SqlQUery += @" and C.CHSEQ#='" + Dependent + "' ";
            }
            if (FromDateQuery != "")
            {
                SqlQUery += FromDateQuery;
            }
            if (ToDateQuery != "")
            {
                SqlQUery += ToDateQuery;
            }
            return SqlQUery;

        }
        public static string GeTDependentClaims(string SSN, string Seq, string ClaimNumber, DateTime? Fromdate, DateTime? Todate, string Dependent, string SortingColumn, string Orderby, int page, int size)
        {

            var FromDateQuery = "";
            if (Fromdate != null)
            {
                FromDateQuery = @" And (CHPRDY>" + Fromdate.Value.Year + " or  (CHPRDY>=" + Fromdate.Value.Year + " AND CHPRDM>" + Fromdate.Value.Month + ") or (CHPRDY>=" + Fromdate.Value.Year + " AND CHPRDM>=" + Fromdate.Value.Month + " AND CHPRDD>=" + Fromdate.Value.Day + "))";
            }
            var ToDateQuery = "";
            if (Todate != null)
            {
                ToDateQuery = @"  And (CHPRDY<" + Todate.Value.Year + " OR  (CHPRDY<=" + Todate.Value.Year + " AND CHPRDM<" + Todate.Value.Month + ") OR (CHPRDY<=" + Todate.Value.Year + " AND CHPRDM<=" + Todate.Value.Month + " AND CHPRDD<=" + Todate.Value.Day + ")) ";

            }
            var SqlQUery = "SELECT  C.CHEOB# AS EOBNo,CHCLM# as ClaimNumber,P.PRPNAM AS PROVIDER,C.CHDEP#,CASE WHEN C.CHDEP# = 0 THEN REPLACE(EMNAME,'*',',')   ELSE REPLACE(d.DPNAME,'*',',')  END AS ForPerson," +
                       " C.CHCLTP AS ClaimType,C.CHCLM$ AS ClaimAmount,C.CHPAY$  AS Paid,CHCOPA + CHDED$ + CHCO$  MemberPaid,CHPRDY as ClaimYear,CHPRDM as ClaimMonth, CHPRDD as ClaimDate" +
                        " FROM CLMHP C INNER JOIN EMPYP e ON e.EMSSN = C.CHSSN LEFT JOIN DEPNP d  ON d.DPSSN = C.CHSSN AND C.CHDEP# = d.DPSEQ " +
                        " INNER JOIN AMBENDF.PROVP P ON P.PRNUM = C.CHPROV  AND P.PRSEQ# = C.CHSEQ#" +
                         " WHERE CHSSN = '" + SSN + "'  AND C.CHDEP# ='" + Seq + "' AND CHCLM# like '%" + ClaimNumber + "%' ";
            if (FromDateQuery != "")
            {
                SqlQUery += FromDateQuery;
            }
            if (ToDateQuery != "")
            {
                SqlQUery += ToDateQuery;
            }
            if (SortingColumn != "" && Orderby != "")
            {
                if (SortingColumn == "ClaimDate")
                {
                    SqlQUery += @" ORDER BY CHPRDY " + Orderby + ", CHPRDM " + Orderby + ", CHPRDD " + Orderby + " ";

                }
                else if (SortingColumn == "MemberPaid")
                {
                    SqlQUery += @" ORDER BY CHCOPA " + Orderby + ", CHDED$ " + Orderby + ", CHCO$ " + Orderby + " ";
                }
                else
                {
                    SqlQUery += @" ORDER BY " + SortingColumn + " " + Orderby + " ";
                }
            }
            if (page != 0 && size != 0)
            {
                SqlQUery += @" OFFSET (" + page + " -1) * " + size + " ROWS FETCH NEXT " + size + " ROWS ONLY";
            }


            return SqlQUery;
        }

        public static string GetMemberDetailsWIthSSN(string SSN)
        {

            return "SELECT  EMSSN as EMSSN,EMNAME AS Name,EMMEM# AS Id,EMSEX  AS Gender,EMDOBY AS DOBY,EMDOBM " +
                   " AS DOBM,EMDOBD AS DOBD,EMADR1 AS Addr1,EMADR2 AS Addr2,EMADR3 AS Addr3,EMADR4 AS  Addr4" +
                    ",EMCITY AS City,EMST AS State,EMZIP5 AS Zip1,EMZIP4 AS Zip2,EMZIP2 AS Zip3 FROM EMPYP WHERE EMSSN =" + SSN;
        }

        public static string GetDEDMET_OOP_Details(int Currentyear, string EMPSSN, int DEPSEQ)
        {

            string strquery = "";

            /* deductible PPO INDIVIDUAL*/
            strquery = @"
             SELECT * FROM (
             SELECT concat(concat('Deductible ','-PPO'),'- Individual') as Desc,IFNULL(SUM(chded$),0) AS Applied,deinda AS Maximum , deinda - IFNULL(SUM(chded$),0) AS Remaining,DEEFDY
             FROM
             (
             SELECT
             'MD1' AS elclas,
             ELSSN
             FROM
             ELGHP WHERE ELSSN = " + EMPSSN + @" AND ELEPDY <= " + Currentyear + @" AND ELDSEQ = 0
             ORDER BY ELEPDY DESC,ELEPDM DESC,ELEPDD DESC
             FETCH FIRST 1 ROWS ONLY
             ) Eligibies
             LEFT JOIN
             DEDSP DED
             ON
             DED.DEDCDE = Eligibies.ELCLAS
             AND
             DED.DEEFDY <= " + Currentyear + @"
             AND
             DED.DESTAT <> 'D'
             LEFT JOIN
             clmhpmed
             ON
             CHSSN = ELSSN
             and chdep# = " + DEPSEQ + @" and chfrdy = " + Currentyear + @" and chded$ <> 0 and chdrop <> 'D'
             AND CHPPO# IN ('BCBC','BLIND')
             GROUP BY
             deinda, DEEFDY
             ORDER BY DEEFDY DESC
             FETCH FIRST 1 ROWS ONLY
             ) AS DedIndNetwork
             ";

            strquery += "\n";
            strquery += " union all ";
            /* DEDD NON PPO INDIVIDUAL*/
            strquery += @"
             SELECT * FROM (
             SELECT concat(concat('Deductible ','Non-PPO'),'- Individual') as Desc,IFNULL(SUM(chded$),0) AS Applied,deinda AS Maximum , deinda - IFNULL(SUM(chded$),0) AS Remaining,DEEFDY
             FROM
             (
             SELECT
             'MED' AS elclas,
             ELSSN
             FROM
             ELGHP WHERE ELSSN = " + EMPSSN + @" AND ELEPDY <= " + Currentyear + @" AND ELDSEQ = 0
             ORDER BY ELEPDY DESC,ELEPDM DESC,ELEPDD DESC
             FETCH FIRST 1 ROWS ONLY
             ) Eligibies
             LEFT JOIN
             DEDSP DED
             ON
             DED.DEDCDE = Eligibies.ELCLAS
             AND
             DED.DEEFDY <= " + Currentyear + @"
             AND
             DED.DESTAT <> 'D'
             LEFT JOIN
             clmhpmed
             ON
             CHSSN = ELSSN
             and chdep# = " + DEPSEQ + @" and chfrdy = " + Currentyear + @" and chded$ <> 0 and chdrop <> 'D'
             AND CHPPO# NOT IN ('BCBC','BLIND')
             GROUP BY
             deinda, DEEFDY
             ORDER BY DEEFDY DESC
             FETCH FIRST 1 ROWS ONLY
             ) AS DedIndOON
             ";

            strquery += "\n";
            strquery += " union all ";
            /* DEDD PPO Family*/

            strquery += @"
             SELECT * FROM (
             SELECT concat(concat('Deductible ','-PPO'),'- Family') as Desc,IFNULL(SUM(chded$),0) AS Applied,defama AS Maximum , defama - IFNULL(SUM(chded$),0) AS Remaining,DEEFDY
             FROM
             (
             SELECT
             'MD1' AS elclas,
             ELSSN
             FROM
             ELGHP WHERE ELSSN = " + EMPSSN + @" AND ELEPDY <= " + Currentyear + @" AND ELDSEQ = 0
             ORDER BY ELEPDY DESC,ELEPDM DESC,ELEPDD DESC
             FETCH FIRST 1 ROWS ONLY
             ) Eligibies
             LEFT JOIN
             DEDSP DED
             ON
             DED.DEDCDE = Eligibies.ELCLAS
             AND
             DED.DEEFDY <= " + Currentyear + @"
             AND
             DED.DESTAT <> 'D'
             LEFT JOIN
             clmhpmed
             ON
             CHSSN = ELSSN
             and chfrdy = " + Currentyear + @" and chded$ <> 0 and chdrop <> 'D'
             AND CHPPO# IN ('BCBC','BLIND')
             GROUP BY
             defama , DEEFDY
             ORDER BY DEEFDY DESC
             FETCH FIRST 1 ROWS ONLY
             ) AS DedIndNetwork";

            strquery += "\n";
            strquery += " union all ";
            /* DEDD NON PPO Family*/

            strquery += @"
             
             SELECT * FROM (
             SELECT concat(concat('Deductible ','Non-PPO'),'- Family') as Desc,IFNULL(SUM(chded$),0) AS Applied,defama AS Maximum , defama - IFNULL(SUM(chded$),0) AS Remaining,DEEFDY
             FROM
             (
             SELECT
             'MED' AS elclas,
             ELSSN
             FROM
             ELGHP WHERE ELSSN = " + EMPSSN + @" AND ELEPDY <= " + Currentyear + @" AND ELDSEQ = 0
             ORDER BY ELEPDY DESC,ELEPDM DESC,ELEPDD DESC
             FETCH FIRST 1 ROWS ONLY
             ) Eligibies
             LEFT JOIN
             DEDSP DED
             ON
             DED.DEDCDE = Eligibies.ELCLAS
             AND
             DED.DEEFDY <= " + Currentyear + @"
             AND
             DED.DESTAT <> 'D'
             LEFT JOIN
             clmhpmed
             ON
             CHSSN = ELSSN
             and chfrdy = " + Currentyear + @" and chded$ <> 0 and chdrop <> 'D'
             AND CHPPO# NOT IN ('BCBC','BLIND')
             GROUP BY
             defama, DEEFDY
             ORDER BY DEEFDY DESC
             FETCH FIRST 1 ROWS ONLY
             ) AS DedIndOON";

            strquery += "\n";
            strquery += " union all ";
            /* OOP PPO Individual*/

            strquery += @" SELECT * FROM (
             SELECT concat(concat('OOP ','-PPO'),'- Individual') as Desc,IFNULL(SUM(chco$),0)+IFNULL(SUM(chded$),0) AS Applied,opopit AS Maximum , opopit - (IFNULL(SUM(chco$),0)+IFNULL(SUM(chded$),0)) AS Remaining,OPEFDY
             FROM
             (
             SELECT
             'MD1' AS elclas,
             ELSSN
             FROM
             ELGHP WHERE ELSSN = " + EMPSSN + @" AND ELEPDY <= " + Currentyear + @" AND ELDSEQ = 0
             ORDER BY ELEPDY DESC,ELEPDM DESC,ELEPDD DESC
             FETCH FIRST 1 ROWS ONLY
             ) Eligibies
             LEFT JOIN
             OOPSP o
             ON
             o.OPOCDE = Eligibies.ELCLAS
             AND
             o.OPEFDY <= " + Currentyear + @"
             AND
             o.OPSTAT <> 'D'
             LEFT JOIN
             clmhpmed
             ON
             CHSSN = ELSSN
             and chdep# = " + DEPSEQ + @" and chfrdy = " + Currentyear + @" and chdrop <> 'D'
             AND CHPPO# IN ('BCBC','BLIND')
             GROUP BY
             opopit,OPEFDY
             ORDER BY OPEFDY DESC
             FETCH FIRST 1 ROWS ONLY
             ) AS OOPIndInNetwork";

            strquery += "\n";
            strquery += " union all ";
            /* OOP non PPO Individual*/

            strquery += @"
             SELECT * FROM (
             SELECT concat(concat('OOP ','Non-PPO'),'- Individual') as Desc,IFNULL(SUM(chco$),0)+IFNULL(SUM(chded$),0) AS Applied,opopit AS Maximum , opopit - (IFNULL(SUM(chco$),0)+IFNULL(SUM(chded$),0)) AS Remaining,OPEFDY
             FROM
             (
             SELECT
             'MED' AS elclas,
             ELSSN
             FROM
             ELGHP WHERE ELSSN = " + EMPSSN + @" AND ELEPDY <= " + Currentyear + @" AND ELDSEQ = 0
             ORDER BY ELEPDY DESC,ELEPDM DESC,ELEPDD DESC
             FETCH FIRST 1 ROWS ONLY
             ) Eligibies
             LEFT JOIN
             OOPSP o
             ON
             o.OPOCDE = Eligibies.ELCLAS
             AND
             o.OPEFDY <= " + Currentyear + @"
             AND
             o.OPSTAT <> 'D'
             LEFT JOIN
             clmhpmed
             ON
             CHSSN = ELSSN
             and chdep# = " + DEPSEQ + @" and chfrdy = " + Currentyear + @" and chdrop <> 'D'
             AND CHPPO# NOT IN ('BCBC','BLIND')
             GROUP BY
             opopit,OPEFDY
             ORDER BY OPEFDY DESC
             FETCH FIRST 1 ROWS ONLY
             
             ) AS OOPIndOOPNetwork
             ";

            strquery += "\n";
            strquery += " union all ";
            /* OOP PPO Family*/

            strquery += @" SELECT * FROM (
             SELECT concat(concat('OOP ','-PPO'),'- Family') as Desc,IFNULL(SUM(chco$),0)+IFNULL(SUM(chded$),0) AS Applied,opopft AS Maximum , opopft - (IFNULL(SUM(chco$),0)+IFNULL(SUM(chded$),0)) AS Remaining,OPEFDY
             FROM
             (
             SELECT
             'MD1' AS elclas,
             ELSSN
             FROM
             ELGHP WHERE ELSSN = " + EMPSSN + @" AND ELEPDY <= " + Currentyear + @" AND ELDSEQ = 0
             ORDER BY ELEPDY DESC,ELEPDM DESC,ELEPDD DESC
             FETCH FIRST 1 ROWS ONLY
             ) Eligibies
             LEFT JOIN
             OOPSP o
             ON
             o.OPOCDE = Eligibies.ELCLAS
             AND
             o.OPEFDY <= " + Currentyear + @"
             AND
             o.OPSTAT <> 'D'
             LEFT JOIN
             clmhpmed
             ON
             CHSSN = ELSSN
             and chfrdy = " + Currentyear + @" and chdrop <> 'D'
             AND CHPPO# IN ('BCBC','BLIND')
             GROUP BY
             opopft,OPEFDY
             ORDER BY OPEFDY DESC
             FETCH FIRST 1 ROWS ONLY
             ) AS OOPIndInNetwork
             ";

            strquery += "\n";
            strquery += " union all ";
            /* OOP non PPO Family*/

            strquery += @"
             SELECT * FROM (
             SELECT concat(concat('OOP ','Non-PPO'),'- Family') as Desc,IFNULL(SUM(chco$),0)+IFNULL(SUM(chded$),0) AS Applied,opopft AS Maximum , opopft - (IFNULL(SUM(chco$),0)+IFNULL(SUM(chded$),0)) AS Remaining,OPEFDY
             FROM
             (
             SELECT
             'MED' AS elclas,
             ELSSN
             FROM
             ELGHP WHERE ELSSN = " + EMPSSN + @" AND ELEPDY <= " + Currentyear + @" AND ELDSEQ = 0
             ORDER BY ELEPDY DESC,ELEPDM DESC,ELEPDD DESC
             FETCH FIRST 1 ROWS ONLY
             ) Eligibies
             LEFT JOIN
             OOPSP o
             ON
             o.OPOCDE = Eligibies.ELCLAS
             AND
             o.OPEFDY <= " + Currentyear + @"
             AND
             o.OPSTAT <> 'D'
             LEFT JOIN
             clmhpmed
             ON
             CHSSN = ELSSN
             and chfrdy = " + Currentyear + @" and chdrop <> 'D'
             AND CHPPO# NOT IN ('BCBC','BLIND')
             GROUP BY
             opopft,OPEFDY
             ORDER BY OPEFDY DESC
             FETCH FIRST 1 ROWS ONLY
             
             ) AS OOPIndOOPNetwork";
            return strquery;
        }

        public static string GetABC_UserFundList(long UserId)
        {
            return @"select ca.CLIENT,FUNDDS,lo.Headerlogo from [dbo].[User_Funds] Uf
                       inner join [BICC_REPORTING].[dbo].[CLIENTS_ABC] CA on uf.Fund=CA.Client
					   inner join [LayoutDetails] lo on lo.CLIENT=uf.Fund
                       where UserId=" + UserId;
        }

        public static string GetClaimDetailsWIthClaimNumber(string SSN, string ClaimNumber)
        {

            return @"SELECT CDCLM# AS ClaimNo,CDLIN# AS LineNo,CDDTST AS Status,CDBNCD as BenefitCode,CDCPT# AS CPT#,CDCHG$ AS TotalCharge,
                      CDDED$ Dedcutible,CDPAY$ Paid,CDCOIN Coinsurance
                  ,CDOOP$ OOP,CDPDSC ProviderDiscount FROM CLMDP WHERE CDSSN = " + SSN + " AND CDCLM# = " + ClaimNumber;

        }

        public static string GetDeductibleMax(string Code, string EffectivePeriod)
        {
            string sQuery = @"SELECT DEINDA AS IND_DED_MAX,DEFAMA AS FAM_DED_MAX 
                              FROM DEDSP DS
                              WHERE (DS.DEDCDE = '" + Code + @"') 
                              AND 
                              (CAST(DEEFDY || '-' || DEEFDM || '-' || DEEFDD AS DATE) = (SELECT MAX(CAST(D2.DEEFDY || '-' || D2.DEEFDM || '-' || D2.DEEFDD AS DATE)) AS MAX_DATE 
	                          FROM DEDSP AS D2 
	                          WHERE (D2.DEDCDE = '" + Code + @"') 
	                          AND 
		                      (D2.DEEFDY <= " + EffectivePeriod + ")) )";

            return sQuery;
        }


        public static string GetDeductibleMet(string Code, string EffectiveYear, string FamilyCode, int SSN)
        {
            string sQuery = @"SELECT ADDED$ AS DED_MET FROM DEDAP WHERE (ADPSSN = " + SSN.ToString() + @") 
                            AND(ADDEPN = 0)

                            AND ( ADEFDY = " + EffectiveYear + @")

                            AND (ADFAMC = '" + FamilyCode + @"')

                            AND(ADACCD = '" + Code + "')";

            return sQuery;
        }


        public static string GetOOPMax(string Code, string EffectivePeriod)
        {
            string sQuery = @"SELECT OS.OPOPIT AS IND_OOP_MAX,OPOPFT AS FAM_OOP_MAX 
                              FROM OOPSP OS
                                WHERE (OS.OPOCDE = '" + Code + @"') 
                                AND 
                                (CAST(OPEFDY || '-' || OPEFDM || '-' || OPEFDD AS DATE) = (SELECT MAX(CAST(D2.OPEFDY || '-' || D2.OPEFDM || '-' || D2.OPEFDD AS DATE)) AS MAX_DATE 
                                FROM OOPSP  AS D2 
                                WHERE (D2.OPOCDE = '" + Code + @"') 
	                                AND 
                                   (D2.OPEFDY  <= '" + EffectivePeriod + "')) )";

            return sQuery;
        }


        public static string GetOOPMet(string Code, string EffectiveYear, string FamilyCode, int SSN)
        {
            string sQuery = @"SELECT AODED$ AS OOP_MET FROM OOPAP o WHERE (AOPSSN = " + SSN.ToString() + @") 
                            AND(AODEPN = 0)

                            AND ( AOEFDY = " + EffectiveYear + @")

                            AND (AOFAMC = '" + FamilyCode + @"')

                            AND(AOACCD = '" + Code + "')";

            return sQuery;
        }


    }
}
