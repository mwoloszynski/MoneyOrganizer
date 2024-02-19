using MoneyOrganizer.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyOrganizer.Model
{
    public class Settings
    {
        public UserInfo userInfo;
        public List<Categories> categories;
        public List<Income> incomes;
        public List<Expenses> expenses;

        public Settings()
        {
            userInfo = new UserInfo();
            categories = new List<Categories>();
            incomes = new List<Income>();
            expenses = new List<Expenses>();
        }

        public void LoadSettings(DBController dbc)
        {
            if(!string.IsNullOrEmpty(dbc.SelectSingleColumnOneResult("UserInfo", "", "Id")))
            {
                userInfo.Id = Int32.Parse(dbc.SelectSingleColumnOneResult("UserInfo", "", "Id"));
                userInfo.FirstName = dbc.SelectSingleColumnOneResult("UserInfo", "", "FirstName");
                userInfo.LastName = dbc.SelectSingleColumnOneResult("UserInfo", "", "LastName");
                userInfo.Balance = float.Parse(dbc.SelectSingleColumnOneResult("UserInfo", "", "Balance"));
            }
            int categoriesCount = dbc.SelectSingleColumnMany("Categories", "", "Id").Count;
            if (categoriesCount >= 1)
            {
                List<string> Id = dbc.SelectSingleColumnMany("Categories", "", "Id");
                List<string> Name = dbc.SelectSingleColumnMany("Categories", "", "Name");
                List<string> Color = dbc.SelectSingleColumnMany("Categories", "", "Color");
                List<string> Priority = dbc.SelectSingleColumnMany("Categories", "", "Priority");
                List<string> ImageUrl = dbc.SelectSingleColumnMany("Categories", "", "ImageUrl");
                for(int i=0; i<categoriesCount; i++)
                {
                    categories.Add(new Categories
                    {
                        Id = Int32.Parse(Id[i]),
                        Name = Name[i],
                        Color = Color[i],
                        Priority = Int32.Parse(Priority[i]),
                        ImageUrl = ImageUrl[i]
                    });
                }
            }
            int incomesCount = dbc.SelectSingleColumnMany("Income", "", "Id").Count;
            if(incomesCount >= 1)
            {
                List<string> Id = dbc.SelectSingleColumnMany("Income", "", "Id");
                List<string> Name = dbc.SelectSingleColumnMany("Income", "", "Name");
                List<string> Amount = dbc.SelectSingleColumnMany("Income", "", "Amount");
                List<string> Date = dbc.SelectSingleColumnMany("Income", "", "Date");
                for(int i=0; i<incomesCount; i++)
                {
                    incomes.Add(new Income
                    {
                        Id = Int32.Parse(Id[i]),
                        Name = Name[i],
                        Amount = float.Parse(Amount[i]),
                        Date = Date[i]
                    });
                }
            }
            int outcomesCount = dbc.SelectSingleColumnMany("Expenses", "", "Id").Count;
            if(outcomesCount >= 1)
            {
                List<string> Id = dbc.SelectSingleColumnMany("Expenses", "", "Id");
                List<string> CategoryId = dbc.SelectSingleColumnMany("Expenses", "", "CategoryId");
                List<string> Name = dbc.SelectSingleColumnMany("Expenses", "", "Name");
                List<string> Cost = dbc.SelectSingleColumnMany("Expenses", "", "Cost");
                List<string> Date = dbc.SelectSingleColumnMany("Expenses", "", "Date");
                for(int i=0; i<outcomesCount; i++)
                {
                    expenses.Add(new Expenses
                    {
                        Id = Int32.Parse(Id[i]),
                        CategoryId = Int32.Parse(CategoryId[i]),
                        Name = Name[i],
                        Cost = float.Parse(Cost[i]),
                        Date = Date[i]
                    });
                }
            }
        }

    }
}
