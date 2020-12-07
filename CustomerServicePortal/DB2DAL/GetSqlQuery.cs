using System;
using System.Linq;

namespace CustomerServicePortal
{
    public static class GetSqlQuery
    {
        public static string GetEMployDetails(string SearchMember, int page, int size)
        {
            if (SearchMember == "")
            {
                return @"SELECT EMSSN as SSN,EMNAME as Member,EMMEM# as ID,EMCITY as City,EMST as State,EMDOBY as Year,EMDOBM as Month,EMDOBD as Day 
                                     FROM EMPYP where EMDROP<> 'D' 
                                   OFFSET (" + page + " -1) * " + size + " ROWS FETCH NEXT " + size + " ROWS ONLY";
            }
            else
            {
                if ((SearchMember).All(char.IsNumber))
                {
                    return @"SELECT EMSSN as SSN,EMNAME as Member,EMMEM# as ID,EMCITY as City,EMST as State,EMDOBY as Year,EMDOBM as Month,EMDOBD as Day
                 FROM EMPYP where EMDROP<> 'D' and (EMSSN =" + SearchMember + " OR EMMEM# =" + SearchMember + ")  OFFSET (" + page + " -1) * " + size + " ROWS FETCH NEXT " + size + " ROWS ONLY";
                }
                else
                {
                    return  @"SELECT EMSSN as SSN,EMNAME as Member,EMMEM# as ID,EMCITY as City,EMST as State,EMDOBY as Year,EMDOBM as Month,EMDOBD as Day
                    FROM EMPYP where EMDROP<> 'D' and (EMNAME LIKE '%" + SearchMember + "%' or UPPER(EMNAME) LIKE '%" + SearchMember + "%' or LOWER(EMNAME) LIKE '%" + SearchMember + "%')   OFFSET (" + page + " -1) * " + size + " " +"ROWS FETCH NEXT " + size + " ROWS ONLY";
                }
            }
        }

        public static string GetDependentDetails(string SSN)
        {
            return @"SELECT DPSSN AS DPSSN,DPSEQ AS SEQ, DPNAME AS NAME,d.DPRLTN AS RELATION,DPSTAT AS STATUS,DPDOBY AS DOBY,DPDOBM AS DOBM ,DPDOBD AS DOBD,
                    d.DPCLAS CLASS, d.DPPLAN AS  PLAN FROM DEPNP d 
                        WHERE DPSSN = '" + SSN + "'   AND DPDROP<> 'D'";
        }

        public  static string GetDependentDetailsWithSeq(string SSN,int DPSEQ)
        {
            return @"SELECT DPSSN AS DPSSN,DPSEQ AS SEQ, DPNAME AS NAME,d.DPRLTN AS RELATION,DPSTAT AS STATUS,DPDOBY AS DOBY,DPDOBM AS DOBM ,DPDOBD AS DOBD,
                    d.DPCLAS CLASS, d.DPPLAN AS  PLAN FROM DEPNP d 
                        WHERE DPSSN = '" + SSN + "' AND DPSEQ =" + DPSEQ + "  AND DPDROP<> 'D'";

        }

        public static string TotalMemeberCount(string SearchMember)
        {
            if (SearchMember == "")
            {
                return "select Count(*) as Total from EMPYP where EMDROP<> 'D'";
            }
            else
            {
                if ((SearchMember).All(char.IsNumber))
                {
                    return "select Count(*) as Total from EMPYP where EMDROP<> 'D' and (EMSSN =" + SearchMember + " OR EMMEM# =" + SearchMember + ")";
                }
                else
                {
                    return "select Count(*) as Total from EMPYP where EMDROP<> 'D' and (EMNAME LIKE '%" + SearchMember + "%' or UPPER(EMNAME) LIKE '%" + SearchMember + "%' or LOWER(EMNAME) LIKE '%" + SearchMember + "%')";
                }
            }
        }

        public static string GeTMemberClaims(string SSN, string ClaimNumber, DateTime? Fromdate, DateTime? Todate)
        {
            var FromDateQuery = "";
            if (Fromdate != null)
            {
                FromDateQuery = " And CHFRDY>=" + Fromdate.Value.Year + " AND  CHFRDM>=" + Fromdate.Value.Month + " AND CHFRDD>=" + Fromdate.Value.Day;
            }
            var ToDateQuery = "";
            if (Todate != null)
            {
                ToDateQuery = " And CHFRDY<=" + Todate.Value.Year + " AND  CHFRDM<=" + Todate.Value.Year + " AND CHFRDD<=" + Todate.Value.Year;
            }
            var SqlQUery = "SELECT C.CHEOB# AS EOBNo,CHCLM# as ClaimNumber,P.PRPNAM AS PROVIDER, C.CHSEQ#,	CASE WHEN C.CHDEP# = 0 THEN E.EMNAME ELSE d.DPNAME END AS ForPerson," +
                     " C.CHCLTP AS ClaimType,C.CHCLM$ AS ClaimAmount,C.CHPAY$  AS Paid,CHCOPA + CHDED$ + CHCO$  MemberPaid,CHPRDY as ClaimYear,CHPRDM ClaimMonth, CHPRDD ClaimDate" +
                       " FROM CLMHP C INNER JOIN EMPYP e ON e.EMSSN = C.CHSSN LEFT JOIN DEPNP d  ON d.DPSSN = C.CHSSN  AND C.CHSEQ# = d.DPSEQ" +
                        " INNER JOIN AMBENDF.PROVP P ON P.PRNUM = C.CHPROV  AND P.PRSEQ# = C.CHSEQ#" +
                          " WHERE CHSSN = '" + SSN + "'  AND C.CHDEP# =" + 0 + " AND CHCLM# like '%" + ClaimNumber + "%'  ORDER BY CHPRDY DESC ,CHPRDM DESC,CHPRDD DESC  ";
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

        public static string GeTDependentClaims(string SSN, string Seq, string ClaimNumber, DateTime? Fromdate, DateTime? Todate)
        {
            var FromDateQuery = "";
            if (Fromdate != null)
            {
                FromDateQuery = " And CHFRDY>=" + Fromdate.Value.Year + " AND  CHFRDM>=" + Fromdate.Value.Month + " AND CHFRDD>=" + Fromdate.Value.Day;
            }
            var ToDateQuery = "";
            if (Todate != null)
            {
                ToDateQuery = " And CHFRDY<=" + Todate.Value.Year + " AND  CHFRDM<=" + Todate.Value.Year + " AND CHFRDD<=" + Todate.Value.Year;
            }
            var SqlQUery = "SELECT  C.CHEOB# AS EOBNo,CHCLM# as ClaimNumber,P.PRPNAM AS PROVIDER,C.CHDEP#,CASE WHEN C.CHDEP# = 0 THEN E.EMNAME ELSE d.DPNAME END AS ForPerson," +
                       " C.CHCLTP AS ClaimType,C.CHCLM$ AS ClaimAmount,C.CHPAY$  AS Paid,CHCOPA + CHDED$ + CHCO$  MemberPaid,CHPRDY as ClaimYear,CHPRDM as ClaimMonth, CHPRDD as ClaimDate" +
                        " FROM CLMHP C INNER JOIN EMPYP e ON e.EMSSN = C.CHSSN LEFT JOIN DEPNP d  ON d.DPSSN = C.CHSSN AND C.CHDEP# = d.DPSEQ " +
                        " INNER JOIN AMBENDF.PROVP P ON P.PRNUM = C.CHPROV  AND P.PRSEQ# = C.CHSEQ#" +
                         " WHERE CHSSN = '" + SSN + "'  AND C.CHDEP# ='" + Seq + "' AND CHCLM# like '%" + ClaimNumber + "%' ORDER BY CHPRDY DESC ,CHPRDM DESC,CHPRDD DESC  ";
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

        public static string GetMemberDetailsWIthSSN(string SSN)
        {
            return "SELECT  EMSSN as EMSSN,EMNAME AS Name,EMMEM# AS Id,EMSEX  AS Gender,EMDOBY AS DOBY,EMDOBM " +
                   " AS DOBM,EMDOBD AS DOBD,EMADR1 AS Addr1,EMADR2 AS Addr2,EMADR3 AS Addr3,EMADR4 AS  Addr4"+
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
    }
}