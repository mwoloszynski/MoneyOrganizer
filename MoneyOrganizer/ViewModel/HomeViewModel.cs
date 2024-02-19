using MoneyOrganizer.Model;
using MoneyOrganizer.Model.DBModel;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MoneyOrganizer.ViewModel
{
    public class HomeViewModel : ViewModelBase
    {

        #region Fields

        private MainViewModel _mvm;
        private ICommand _addIncomeCommand;
        private ICommand _addOutcomeCommand;
        private List<ExpensesChart> _expensesChartTest;
        private List<Transaction> _transactions;
        private List<Expenses> _expensesHistory;
        private float _balance;
        private float _chartMin;
        private float _chartMax;

        #endregion

        #region Binds

        public ICommand AddIncomeCommand { get { return _addIncomeCommand; } }
        public ICommand AddOutcomeCommand { get { return _addOutcomeCommand; } }
        public List<ExpensesChart> ExpensesChartTest
        {
            get
            {
                return _expensesChartTest;
            }
            set
            {
                _expensesChartTest = value;
                OnPropertyChanged("ExpensesChartTest");
            }
        }
        public List<Transaction> Transactions
        {
            get
            {
                return _transactions;
            }
            set
            {
                _transactions = value;
                OnPropertyChanged("Transactions");
            }
        }
        public List<Expenses> ExpensesHistory
        {
            get
            {
                return _expensesHistory;
            }
            set
            {
                _expensesHistory = value;
                OnPropertyChanged("ExpensesHistory");
            }
        }
        public float Balance
        {
            get
            {
                return _balance;
            }
            set
            {
                _balance = value;
                OnPropertyChanged("Balance");
            }
        }
        public float ChartMin
        {
            get
            {
                return _chartMin;
            }
            set
            {
                _chartMin = value;
                OnPropertyChanged("ChartMin");
            }
        }
        public float ChartMax
        {
            get
            {
                return _chartMax;
            }
            set
            {
                _chartMax = value;
                OnPropertyChanged("ChartMax");
            }
        }

        #endregion

        #region Constructor

        public HomeViewModel(MainViewModel mvm)
        {
            _mvm = mvm;
            _addIncomeCommand = new RelayCommand(_addIncomeCommand => AddIncome());
            _addOutcomeCommand = new RelayCommand(_addOutcomeCommand => AddOutcome());
            ExpensesChartTest = new List<ExpensesChart>();
            Transactions = new List<Transaction>();
            _balance = _mvm.AppSettings.userInfo.Balance;
            _expensesHistory = new List<Expenses>();
            _expensesHistory = _mvm.AppSettings.expenses;
            _chartMin = 0f;
            _chartMax = 100f;

            SetTransactions();
            SetChart();
           
        }

        #endregion

        #region Functions

        public void SetChart()
        {
            if (ExpensesHistory.Count > 0)
            {
                ExpensesHistory = ExpensesHistory.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).ToList();
                var DateTemp = ExpensesHistory[0].Date;
                var DateCounter = 0;
                float amount = 0f;
                int i = 0;
                while (DateCounter != 7 && i < ExpensesHistory.Count)
                {
                    amount = 0f;
                    while (i < ExpensesHistory.Count && ExpensesHistory[i].Date == DateTemp)
                    {
                        amount += ExpensesHistory[i].Cost;
                        i++;
                    }
                    DateCounter++;
                    ExpensesChartTest.Add(new ExpensesChart { Name = DateTemp, Amount = amount });
                    if (i < ExpensesHistory.Count)
                        DateTemp = ExpensesHistory[i].Date;
                }
                var chartmaximum = ExpensesChartTest.OrderByDescending(x => x.Amount).ElementAt(0).Amount;
                var chartminimum = ExpensesChartTest.OrderBy(x => x.Amount).ElementAt(0).Amount;

                ChartMax = chartmaximum;

                ExpensesChartTest = ExpensesChartTest.OrderBy(x => x.Name).ToList();
            }
        }

        public void UpdateChart()
        {
            if (ExpensesHistory.Count > 0)
            {
                ExpensesHistory = _mvm.AppSettings.expenses;
                ExpensesHistory = ExpensesHistory.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).ToList();

                ExpensesChartTest = new List<ExpensesChart>();

                var DateTemp = ExpensesHistory[0].Date;
                var DateCounter = 0;
                float amount = 0f;
                int i = 0;
                while (DateCounter != 7 && i < ExpensesHistory.Count)
                {
                    amount = 0f;
                    while (i < ExpensesHistory.Count && ExpensesHistory[i].Date == DateTemp)
                    {
                        amount += ExpensesHistory[i].Cost;
                        i++;
                    }
                    DateCounter++;
                    ExpensesChartTest.Add(new ExpensesChart { Name = DateTemp, Amount = amount });
                    if (i < ExpensesHistory.Count)
                        DateTemp = ExpensesHistory[i].Date;
                }
                var chartmaximum = ExpensesChartTest.OrderByDescending(x => x.Amount).ElementAt(0).Amount;
                var chartminimum = ExpensesChartTest.OrderBy(x => x.Amount).ElementAt(0).Amount;

                ChartMax = chartmaximum;

                ExpensesChartTest = ExpensesChartTest.OrderBy(x => x.Name).ToList();
            }
        }

        public void SetTransactions()
        {
            ExpensesHistory = ExpensesHistory.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).ToList();
            int counter = 0;
            foreach (var expense in ExpensesHistory)
            {
                Transactions.Add(new Transaction
                {
                    Category = _mvm.AppSettings.categories.Where(x => x.Id == expense.CategoryId).FirstOrDefault().Name,
                    CategoryImage = _mvm.AppSettings.categories.Where(x => x.Id == expense.CategoryId).FirstOrDefault().ImageUrl,
                    Name = expense.Name,
                    Amount = expense.Cost,
                    Date = expense.Date
                });
                counter++;
                if (counter >= 5)
                    break;
            }
            
        }

        public void UpdateTransactions()
        {
            ExpensesHistory = _mvm.AppSettings.expenses;
            ExpensesHistory = ExpensesHistory.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).ToList();

            Transactions = new List<Transaction>();

            int counter = 0;
            foreach (var expense in ExpensesHistory)
            {
                Transactions.Add(new Transaction
                {
                    Category = _mvm.AppSettings.categories.Where(x => x.Id == expense.CategoryId).FirstOrDefault().Name,
                    CategoryImage = _mvm.AppSettings.categories.Where(x => x.Id == expense.CategoryId).FirstOrDefault().ImageUrl,
                    Name = expense.Name,
                    Amount = expense.Cost,
                    Date = expense.Date
                });
                counter++;
                if (counter >= 5)
                    break;
            }
        }

        private void AddIncome()
        {
            View.AddEarningView earningView = new View.AddEarningView()
            {
                DataContext = new AddEarningViewModel(_mvm)
            };
            earningView.ShowDialog();
            _mvm.NagivateHome();
        }

        private void AddOutcome()
        {
            View.AddExpenseView expenseView = new View.AddExpenseView()
            {
                DataContext = new AddExpenseViewModel(_mvm)
            };
            expenseView.ShowDialog();
            _mvm.NagivateHome();
        }

        #endregion
    }

    public class ExpensesChart
    {
        public string Name { get; set; }
        public float Amount { get; set; }
    }
}
