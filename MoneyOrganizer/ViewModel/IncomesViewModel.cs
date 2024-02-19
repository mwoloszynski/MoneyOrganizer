using MoneyOrganizer.Model;
using MoneyOrganizer.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MoneyOrganizer.ViewModel
{
    public class IncomesViewModel : ViewModelBase
    {
        #region Fields

        private MainViewModel _mvm;
        private ICommand _addIncomeCommand;
        private ICommand _lastWeekFiltrCommand;
        private ICommand _lastMonthFiltrCommand;
        private ICommand _timeIntervalFiltrCommand;
        private ICommand _deleteIncomeCommand;
        private ICommand _generateRaportCommand;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private List<Transaction> _transactions;
        private List<Income> _incomeHistory;

        #endregion

        #region Binds

        public ICommand AddIncomeCommand { get { return _addIncomeCommand; } }
        public ICommand LastWeekFiltrCommand { get { return _lastWeekFiltrCommand; } }
        public ICommand LastMonthFiltrCommand { get { return _lastMonthFiltrCommand; } }
        public ICommand TimeIntervalFiltrCommand { get { return _timeIntervalFiltrCommand; } }
        public ICommand DeleteIncomeCommand { get { return _deleteIncomeCommand; } }
        public ICommand GenerateRaportCommand { get { return _generateRaportCommand; } }
        public DateTime DateFrom
        {
            get
            {
                return _dateFrom;
            }
            set
            {
                _dateFrom = value;
                OnPropertyChanged("DateFrom");
            }
        }
        public DateTime DateTo
        {
            get
            {
                return _dateTo;
            }
            set
            {
                _dateTo = value;
                OnPropertyChanged("DateTo");
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
        public List<Income> IncomeHistory
        {
            get
            {
                return _incomeHistory;
            }
            set
            {
                _incomeHistory = value;
                OnPropertyChanged("IncomeHistory");
            }
        }

        #endregion

        #region Constructor

        public IncomesViewModel(MainViewModel mvm)
        {
            _mvm = mvm;
            _addIncomeCommand = new RelayCommand(_addIncomeCommand => AddIncome());
            _lastWeekFiltrCommand = new RelayCommand(_lastWeekFiltrCommand => LastWeekFiltr());
            _lastMonthFiltrCommand = new RelayCommand(_lastMonthFiltrCommand => LastMonthFiltr());
            _timeIntervalFiltrCommand = new RelayCommand(_timeIntervalFiltrCommand => TimeIntervalFiltr());
            _deleteIncomeCommand = new RelayCommand(_deleteIncomeCommand => DeleteIncome(_deleteIncomeCommand));
            _generateRaportCommand = new RelayCommand(_generateRaportCommand => GenerateRaport());
            _dateFrom = DateTime.Today;
            _dateTo = DateTime.Today;
            Transactions = new List<Transaction>();
            _incomeHistory = new List<Income>();
            _incomeHistory = _mvm.AppSettings.incomes;

            SetTransactions();
        }

        #endregion

        #region Functions

        private void DeleteIncome(object parameter)
        {
            string name = "Income";
            int id = Int32.Parse(parameter.ToString());
            float amount = IncomeHistory.Where(x => x.Id == id).FirstOrDefault().Amount;
            string column = "Id";
            string value = parameter.ToString();
            if (_mvm.AppSettings.userInfo.Balance - amount < 0)
            {
                MessageBox.Show("Budżet spadł poniżej 0");
            }
            _mvm.DBC.DeleteSingleRow(name, column, value);

            string tableName = "UserInfo";
            string[] columns = { "Balance" };
            string[] values = { ((float)Math.Ceiling((_mvm.AppSettings.userInfo.Balance - amount) * 100f) / 100f).ToString() };
            string[] colCond = { "Id" };
            string[] valCond = { _mvm.AppSettings.userInfo.Id.ToString() };
            string[] eqCond = { "=" };
            string[] andor = { "" };
            _mvm.DBC.UpdateTable(tableName, columns, values, colCond, valCond, eqCond, andor);

            _mvm.AppSettings.incomes.Remove(_mvm.AppSettings.incomes.Where(x => x.Id == id).FirstOrDefault());
            IncomeHistory.Remove(IncomeHistory.Where(x => x.Id == id).FirstOrDefault());

            float newAmount = (float)Math.Ceiling((_mvm.AppSettings.userInfo.Balance - amount) * 100f) / 100f;
            _mvm.AppSettings.userInfo.Balance = newAmount;

            _mvm.NavigateIncomes();

        }

        public void UpdateTransactions()
        {
            Transactions = new List<Transaction>();
            _incomeHistory = new List<Income>();
            _incomeHistory = _mvm.AppSettings.incomes;
            SetTransactions();
        }

        public void SetTransactions()
        {
            IncomeHistory = IncomeHistory.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).ToList();
            foreach (var income in IncomeHistory)
            {
                Transactions.Add(new Transaction
                {
                    Id = income.Id,
                    Name = income.Name,
                    Amount = income.Amount,
                    Date = income.Date
                });
            }

        }

        private void GenerateRaport()
        {

        }

        private void LastWeekFiltr()
        {
            Transactions = new List<Transaction>();
            SetTransactions();
            DateTime today = DateTime.Today;
            DateTime lastWeek = today.AddDays(-7);
            today = today.AddDays(1);
            Transactions = Transactions.Where(x => DateTime.Parse(x.Date) >= lastWeek && DateTime.Parse(x.Date) <= today).ToList();
        }

        private void LastMonthFiltr()
        {
            Transactions = new List<Transaction>();
            SetTransactions();
            DateTime today = DateTime.Today;
            DateTime lastMonth = today.AddMonths(-1);
            today = today.AddDays(1);
            Transactions = Transactions.Where(x => DateTime.Parse(x.Date) >= lastMonth && DateTime.Parse(x.Date) <= today).ToList();
        }

        private void TimeIntervalFiltr()
        {
            Transactions = new List<Transaction>();
            SetTransactions();
            DateTo = DateTo.AddDays(1);
            Transactions = Transactions.Where(x => DateTime.Parse(x.Date) >= DateFrom && DateTime.Parse(x.Date) <= DateTo).ToList();
            DateTo = DateTo.AddDays(-1);
        }

        private void AddIncome()
        {
            View.AddEarningView earningView = new View.AddEarningView()
            {
                DataContext = new AddEarningViewModel(_mvm)
            };
            earningView.ShowDialog();
            _mvm.NavigateIncomes();
        }

        #endregion
    }

}
