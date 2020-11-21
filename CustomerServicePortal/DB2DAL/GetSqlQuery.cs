using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerServicePortal
{
    public static class GetSqlQuery
    {
        public static string GetEMployDetails(string SearchMember,int page,int size)
        {
            if (SearchMember=="")
            {
                return "SELECT EMSSN as SSN,EMNAME as Member,EMMEM# as ID,EMCITY as City,EMST as State,EMDOBY as Year,EMDOBM as Month,EMDOBD as Day" +
                                    " FROM EMPYP where EMDROP<> 'D' "+
                                   "OFFSET (" + page + " -1) * " + size + " ROWS FETCH NEXT " + size + " ROWS ONLY";
            }
            else
            {
                if ((SearchMember).All(char.IsNumber))
                {
                    return "SELECT EMSSN as SSN,EMNAME as Member,EMMEM# as ID,EMCITY as City,EMST as State,EMDOBY as Year,EMDOBM as Month,EMDOBD as Day" +
                " FROM EMPYP where EMDROP<> 'D' and (EMSSN =" + SearchMember + " OR EMMEM# =" + SearchMember + ")" +
               "OFFSET (" + page + " -1) * " + size + " ROWS FETCH NEXT " + size + " ROWS ONLY";


                }
                else
                {
                    return "SELECT EMSSN as SSN,EMNAME as Member,EMMEM# as ID,EMCITY as City,EMST as State,EMDOBY as Year,EMDOBM as Month,EMDOBD as Day" +
                   " FROM EMPYP where EMDROP<> 'D' and (EMNAME LIKE '%" + SearchMember + "%' or UPPER(EMNAME) LIKE '%" + SearchMember + "%' or LOWER(EMNAME) LIKE '%" + SearchMember + "%')" +
                   "OFFSET (" + page + " -1) * " + size + " ROWS FETCH NEXT " + size + " ROWS ONLY";

                }

            }
        }
        public static string GetDependentDetails(string SSN)
        {

            return "SELECT DPSSN AS DPSSN,DPSEQ AS SEQ, DPNAME AS NAME,d.DPRLTN AS RELATION,DPSTAT AS STATUS,DPDOBY AS DOBY,DPDOBM AS DOBM ,DPDOBD AS DOBD,"+
	                "d.DPCLAS CLASS, d.DPPLAN AS  PLAN FROM DEPNP d "+
                        " WHERE DPSSN = '"+ SSN + "'  AND DPDROP<> 'D'";
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

        public static string GeTMemberClaims(string SSN,string ClaimNumber,DateTime? Fromdate,DateTime? Todate)
        {
            var FromDateQuery = "";
            if (Fromdate !=null)
            {
                FromDateQuery = " And CHFRDY>="+Fromdate.Value.Year+ " AND  CHFRDM>=" + Fromdate.Value.Month + " AND CHFRDD>=" + Fromdate.Value.Day ;
            }
            var ToDateQuery = "";
            if (Todate != null)
            {
                ToDateQuery = " And CHFRDY<=" + Todate.Value.Year + " AND  CHFRDM<=" + Todate.Value.Year + " AND CHFRDD<=" + Todate.Value.Year;
            }
            var SqlQUery= "SELECT C.CHEOB# AS EOBNo,CHCLM# as ClaimNumber,P.PRPNAM AS PROVIDER, C.CHSEQ#,	CASE WHEN C.CHDEP# = 0 THEN E.EMNAME ELSE d.DPNAME END AS ForPerson," +
                     " C.CHCLTP AS ClaimType,C.CHCLM$ AS ClaimAmount,C.CHPAY$  AS Paid,CHCOPA + CHDED$ + CHCO$  MemberPaid,CHPRDY as ClaimYear,CHPRDM ClaimMonth, CHPRDD ClaimDate" +
                       " FROM CLMHP C INNER JOIN EMPYP e ON e.EMSSN = C.CHSSN LEFT JOIN DEPNP d  ON d.DPSSN = C.CHSSN  AND C.CHSEQ# = d.DPSEQ"+
                        " INNER JOIN AMBENDF.PROVP P ON P.PRNUM = C.CHPROV  AND P.PRSEQ# = C.CHSEQ#" +
                          " WHERE CHSSN = '" + SSN + "'  AND C.CHDEP# =" + 0 + " AND CHCLM# like '%"+ ClaimNumber + "%'";
            if (FromDateQuery !="")
            {
                SqlQUery += FromDateQuery;
            }
            if (ToDateQuery != "")
            {
                SqlQUery += ToDateQuery;
            }
            return SqlQUery;
        }


        public static string GeTDependentClaims(string SSN,string Seq,string ClaimNumber,DateTime? Fromdate,DateTime? Todate)
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
                       " C.CHCLTP AS ClaimType,C.CHCLM$ AS ClaimAmount,C.CHPAY$  AS Paid,CHCOPA + CHDED$ + CHCO$  MemberPaid,CHPRDY as ClaimYear,CHPRDM ClaimMonth, CHPRDD ClaimDate" +
                        " FROM CLMHP C INNER JOIN EMPYP e ON e.EMSSN = C.CHSSN LEFT JOIN DEPNP d  ON d.DPSSN = C.CHSSN AND C.CHDEP# = d.DPSEQ " +
                        " INNER JOIN AMBENDF.PROVP P ON P.PRNUM = C.CHPROV  AND P.PRSEQ# = C.CHSEQ#" +
                         " WHERE CHSSN = '" + SSN+ "'  AND C.CHDEP# ='" + Seq+ "' AND CHCLM# like '%" + ClaimNumber + "%'";
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
                   " AS DOBM,EMDOBD AS DOBD FROM EMPYP WHERE EMSSN ="+ SSN;
        }


    }
}