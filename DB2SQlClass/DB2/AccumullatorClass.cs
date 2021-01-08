using DB2SQlClass.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace DB2SQlClass.DB2
{
    public  static class AccumullatorClass
    {
        public static List<DEDMET_OOP_Model> GetAccumullatorLogic(int Year, List<DEDMET_OOP_Model> dEDMET_OOP_Models, DataTable dtDedScheduled, DataTable dtOOPScheduled, DataTable dtOOPMetFamily, DataTable dtOOPMetIndividual, DataTable dtDedMetIndividual, DataTable dtDedMetFamily)
        {
            // Individual Deductible
            DEDMET_OOP_Model IndividualDeductible = new DEDMET_OOP_Model();
            if (dtDedMetIndividual.Rows.Count > 0)
            {
                IndividualDeductible.APPLIED = ((decimal)dtDedMetIndividual.Rows[0]["DED_MET"]).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                IndividualDeductible.APPLIED = (0).ToString("C", CultureInfo.CurrentCulture);
            }

            if (dtDedScheduled.Rows.Count > 0)
            {
                IndividualDeductible.MAXIMUM = ((decimal)dtDedScheduled.Rows[0]["IND_DED_MAX"]).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                IndividualDeductible.MAXIMUM = (0).ToString("C", CultureInfo.CurrentCulture);
            }

            if ((Convert.ToDecimal(Decimal.Parse(IndividualDeductible.MAXIMUM, NumberStyles.Currency)) - Convert.ToDecimal(Decimal.Parse(IndividualDeductible.APPLIED, NumberStyles.Currency))) >= 0)
            {
                IndividualDeductible.REMAINING = (Convert.ToDecimal(Decimal.Parse(IndividualDeductible.MAXIMUM, NumberStyles.Currency)) - Convert.ToDecimal(Decimal.Parse(IndividualDeductible.APPLIED, NumberStyles.Currency))).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                IndividualDeductible.REMAINING = (0).ToString("C", CultureInfo.CurrentCulture);
            }




            IndividualDeductible.DESC = "Deductible -PPO- Individual";
            IndividualDeductible.Year = Year;

            dEDMET_OOP_Models.Add(IndividualDeductible);

            // Family Deductible
            DEDMET_OOP_Model FamilyDeductible = new DEDMET_OOP_Model();
            if (dtDedMetFamily.Rows.Count > 0)
            {
                FamilyDeductible.APPLIED = ((decimal)dtDedMetFamily.Rows[0]["DED_MET"]).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                FamilyDeductible.APPLIED = (0).ToString("C", CultureInfo.CurrentCulture);
            }
            if (dtDedScheduled.Rows.Count > 0)
            {
                FamilyDeductible.MAXIMUM = ((decimal)dtDedScheduled.Rows[0]["FAM_DED_MAX"]).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                FamilyDeductible.MAXIMUM = (0).ToString("C", CultureInfo.CurrentCulture);
            }

            if ((Convert.ToDecimal(Decimal.Parse(FamilyDeductible.MAXIMUM, NumberStyles.Currency)) - Convert.ToDecimal(Decimal.Parse(FamilyDeductible.APPLIED, NumberStyles.Currency))) >= 0)
            {
                FamilyDeductible.REMAINING = (Convert.ToDecimal(Decimal.Parse(FamilyDeductible.MAXIMUM, NumberStyles.Currency)) - Convert.ToDecimal(Decimal.Parse(FamilyDeductible.APPLIED, NumberStyles.Currency))).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                FamilyDeductible.REMAINING = (0).ToString("C", CultureInfo.CurrentCulture);
            }


            FamilyDeductible.DESC = "Deductible -PPO- Family";
            FamilyDeductible.Year = Year;
            dEDMET_OOP_Models.Add(FamilyDeductible);




            // Individual OOP
            DEDMET_OOP_Model IndividualOOP = new DEDMET_OOP_Model();
            if (dtOOPMetIndividual.Rows.Count > 0)
            {
                IndividualOOP.APPLIED = ((decimal)dtOOPMetIndividual.Rows[0]["OOP_MET"]).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                IndividualOOP.APPLIED = (0).ToString("C", CultureInfo.CurrentCulture);
            }
            if (dtOOPScheduled.Rows.Count > 0)
            {
                IndividualOOP.MAXIMUM = ((decimal)dtOOPScheduled.Rows[0]["IND_OOP_MAX"]).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                IndividualOOP.MAXIMUM = (0).ToString("C", CultureInfo.CurrentCulture);
            }

            if ((Convert.ToDecimal(Decimal.Parse(IndividualOOP.MAXIMUM, NumberStyles.Currency)) - Convert.ToDecimal(Decimal.Parse(IndividualOOP.APPLIED, NumberStyles.Currency))) >= 0)
            {
                IndividualOOP.REMAINING = (Convert.ToDecimal(Decimal.Parse(IndividualOOP.MAXIMUM, NumberStyles.Currency)) - Convert.ToDecimal(Decimal.Parse(IndividualOOP.APPLIED, NumberStyles.Currency))).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                IndividualOOP.REMAINING = (0).ToString("C", CultureInfo.CurrentCulture);
            }

            IndividualOOP.DESC = "OOP -PPO- Individual";
            IndividualOOP.Year = Year;

            dEDMET_OOP_Models.Add(IndividualOOP);

            // Family OOP
            DEDMET_OOP_Model FamilyOOP = new DEDMET_OOP_Model();


            if (dtOOPMetFamily.Rows.Count > 0)
            {
                FamilyOOP.APPLIED = ((decimal)dtOOPMetFamily.Rows[0]["OOP_MET"]).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                FamilyOOP.APPLIED = (0).ToString("C", CultureInfo.CurrentCulture);
            }
            if (dtOOPScheduled.Rows.Count > 0)
            {
                FamilyOOP.MAXIMUM = ((decimal)dtOOPScheduled.Rows[0]["FAM_OOP_MAX"]).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                FamilyOOP.MAXIMUM = (0).ToString("C", CultureInfo.CurrentCulture);
            }

            if ((Convert.ToDecimal(Decimal.Parse(FamilyOOP.MAXIMUM, NumberStyles.Currency)) - Convert.ToDecimal(Decimal.Parse(FamilyOOP.APPLIED, NumberStyles.Currency))) >= 0)
            {
                FamilyOOP.REMAINING = (Convert.ToDecimal(Decimal.Parse(FamilyOOP.MAXIMUM, NumberStyles.Currency)) - Convert.ToDecimal(Decimal.Parse(FamilyOOP.APPLIED, NumberStyles.Currency))).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                FamilyOOP.REMAINING = (0).ToString("C", CultureInfo.CurrentCulture);
            }

            FamilyOOP.DESC = "OOP -PPO- Family";
            FamilyOOP.Year = Year;
            dEDMET_OOP_Models.Add(FamilyOOP);

            return dEDMET_OOP_Models;
        }
    }
}
