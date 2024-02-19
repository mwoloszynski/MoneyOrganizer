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
    public class OutcomesViewModel : ViewModelBase
    {
        #region Fields

        private MainViewModel _mvm;
        private ICommand _addOutcomeCommand;
        private ICommand _lastWeekFiltrCommand;
        private ICommand _lastMonthFiltrCommand;
        private ICommand _timeIntervalFiltrCommand;
        private ICommand _categorySortCommand;
        private ICommand _categoryClearCommand;
        private ICommand _deleteOutcomeCommand;
        private ICommand _generateRaportCommand;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private List<Categories> _categories;
        private Categories _selectedCategory;
        private List<Transaction> _transactions;
        private List<Expenses> _expensesHistory;
        private bool _sorted;
        private int _filtered;

        #endregion

        #region Binds

        public ICommand AddOutcomeCommand { get { return _addOutcomeCommand; } }
        public ICommand LastWeekFiltrCommand { get { return _lastWeekFiltrCommand; } }
        public ICommand LastMonthFiltrCommand { get { return _lastMonthFiltrCommand; } }
        public ICommand TimeIntervalFiltrCommand { get { return _timeIntervalFiltrCommand; } }
        public ICommand CategorySortCommand { get { return _categorySortCommand; } }
        public ICommand CategoryClearCommand { get { return _categoryClearCommand; } }
        public ICommand DeleteOutcomeCommand { get { return _deleteOutcomeCommand; } }
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
        public List<Categories> Category
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                OnPropertyChanged("Category");
            }
        }
        public Categories SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged("SelectedCategory");
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

        #endregion

        #region Constructor

        public OutcomesViewModel(MainViewModel mvm)
        {
            _mvm = mvm;
            _addOutcomeCommand = new RelayCommand(_addOutcomeCommand => AddOutcome());
            _lastWeekFiltrCommand = new RelayCommand(_lastWeekFiltrCommand => LastWeekFiltr());
            _lastMonthFiltrCommand = new RelayCommand(_lastMonthFiltrCommand => LastMonthFiltr());
            _timeIntervalFiltrCommand = new RelayCommand(_timeIntervalFiltrCommand => TimeIntervalFiltr());
            _categorySortCommand = new RelayCommand(_categorySortCommand => CategorySort());
            _categoryClearCommand = new RelayCommand(_categoryClearCommand => CategoryClear());
            _deleteOutcomeCommand = new RelayCommand(_deleteOutcomeCommand => DeleteOutcome(_deleteOutcomeCommand));
            _generateRaportCommand = new RelayCommand(_generateRaportCommand => GenerateRaport());
            _dateFrom = DateTime.Today;
            _dateTo = DateTime.Today;
            _categories = _mvm.AppSettings.categories;
            _selectedCategory = _categories.Where(x => x.Id == 17).FirstOrDefault();
            Transactions = new List<Transaction>();
            _expensesHistory = new List<Expenses>();
            _expensesHistory = _mvm.AppSettings.expenses;
            _filtered = 0;
            _sorted = false;

            SetTransactions();
        }

        #endregion

        #region Functions

        private void DeleteOutcome(object parameter)
        {
            string name = "Expenses";
            int id = Int32.Parse(parameter.ToString());
            float amount = ExpensesHistory.Where(x => x.Id == id).FirstOrDefault().Cost;
            string column = "Id";
            string value = parameter.ToString();
            _mvm.DBC.DeleteSingleRow(name, column, value);

            string tableName = "UserInfo";
            string[] columns = { "Balance" };
            string[] values = { ((float)Math.Ceiling((_mvm.AppSettings.userInfo.Balance + amount) * 100f) / 100f).ToString() };
            string[] colCond = { "Id" };
            string[] valCond = { _mvm.AppSettings.userInfo.Id.ToString() };
            string[] eqCond = { "=" };
            string[] andor = { "" };
            _mvm.DBC.UpdateTable(tableName, columns, values, colCond, valCond, eqCond, andor);

            _mvm.AppSettings.expenses.Remove(_mvm.AppSettings.expenses.Where(x => x.Id == id).FirstOrDefault());
            ExpensesHistory.Remove(ExpensesHistory.Where(x => x.Id == id).FirstOrDefault());

            float newAmount = (float)Math.Ceiling((_mvm.AppSettings.userInfo.Balance + amount) * 100f) / 100f;
            _mvm.AppSettings.userInfo.Balance = newAmount;

            _mvm.NavigateOutcomes();
        }

        public void UpdateTransactions()
        {
            Transactions = new List<Transaction>();
            _expensesHistory = new List<Expenses>();
            _expensesHistory = _mvm.AppSettings.expenses;
            SetTransactions();
        }

        public void SetTransactions()
        {
            ExpensesHistory = ExpensesHistory.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).ToList();
            foreach (var expense in ExpensesHistory)
            {
                Transactions.Add(new Transaction
                {
                    Id = expense.Id,
                    Category = _mvm.AppSettings.categories.Where(x => x.Id == expense.CategoryId).FirstOrDefault().Name,
                    CategoryImage = _mvm.AppSettings.categories.Where(x => x.Id == expense.CategoryId).FirstOrDefault().ImageUrl,
                    Name = expense.Name,
                    Amount = expense.Cost,
                    Date = expense.Date
                });
            }
            _sorted = false;
            _filtered = 0;
        }

        private void GenerateRaport()
        {

        }

        private void LastWeekFiltr()
        {
            if (_sorted)
            {
                Transactions = new List<Transaction>();
                SetTransactions();
                DateTime today = DateTime.Today;
                DateTime lastWeek = today.AddDays(-7);
                today = today.AddDays(1);
                Transactions = Transactions.Where(x => DateTime.Parse(x.Date) >= lastWeek && DateTime.Parse(x.Date) <= today).ToList();
                Transactions = Transactions.Where(x => x.Category == SelectedCategory.Name).ToList();
                _sorted = true;
            }
            else
            {
                Transactions = new List<Transaction>();
                SetTransactions();
                DateTime today = DateTime.Today;
                DateTime lastWeek = today.AddDays(-7);
                today = today.AddDays(1);
                Transactions = Transactions.Where(x => DateTime.Parse(x.Date) >= lastWeek && DateTime.Parse(x.Date) <= today).ToList();
            }
            _filtered = 1;
        }

        private void LastMonthFiltr()
        {
            if (_sorted)
            {
                Transactions = new List<Transaction>();
                SetTransactions();
                DateTime today = DateTime.Today;
                DateTime lastMonth = today.AddMonths(-1);
                today = today.AddDays(1);
                Transactions = Transactions.Where(x => DateTime.Parse(x.Date) >= lastMonth && DateTime.Parse(x.Date) <= today).ToList();
                Transactions = Transactions.Where(x => x.Category == SelectedCategory.Name).ToList();
                _sorted = true;
            }
            else
            {
                Transactions = new List<Transaction>();
                SetTransactions();
                DateTime today = DateTime.Today;
                DateTime lastMonth = today.AddMonths(-1);
                today = today.AddDays(1);
                Transactions = Transactions.Where(x => DateTime.Parse(x.Date) >= lastMonth && DateTime.Parse(x.Date) <= today).ToList();
            }
            _filtered = 2;
        }

        private void TimeIntervalFiltr()
        {
            if (_sorted)
            {
                Transactions = new List<Transaction>();
                SetTransactions();
                DateTo = DateTo.AddDays(1);
                Transactions = Transactions.Where(x => DateTime.Parse(x.Date) >= DateFrom && DateTime.Parse(x.Date) <= DateTo).ToList();
                DateTo = DateTo.AddDays(-1);
                Transactions = Transactions.Where(x => x.Category == SelectedCategory.Name).ToList();
                _sorted = true;
            }
            else
            {
                Transactions = new List<Transaction>();
                SetTransactions();
                DateTo = DateTo.AddDays(1);
                Transactions = Transactions.Where(x => DateTime.Parse(x.Date) >= DateFrom && DateTime.Parse(x.Date) <= DateTo).ToList();
                DateTo = DateTo.AddDays(-1);
            }
            _filtered = 3;
        }

        private void CategorySort()
        {
            if (_filtered != 0)
            {
                _sorted = true;
                switch (_filtered)
                {
                    case 1:
                        LastWeekFiltr();
                        break;
                    case 2:
                        LastMonthFiltr();
                        break;
                    case 3:
                        TimeIntervalFiltr();
                        break;
                }
            }
            else
            {
                Transactions = new List<Transaction>();
                SetTransactions();
                Transactions = Transactions.Where(x => x.Category == SelectedCategory.Name).ToList();
            }
            _sorted = true;
        }

        private void CategoryClear()
        {
            _sorted = false;
            if (_filtered != 0)
            {
                switch(_filtered)
                {
                    case 1:
                        LastWeekFiltr();
                        break;
                    case 2:
                        LastMonthFiltr();
                        break;
                    case 3:
                        TimeIntervalFiltr();
                        break;
                }
            }
            else
            {
                Transactions = new List<Transaction>();
                SetTransactions();
            }
        }

        private void AddOutcome()
        {
            View.AddExpenseView expenseView = new View.AddExpenseView()
            {
                DataContext = new AddExpenseViewModel(_mvm)
            };
            expenseView.ShowDialog();
            _mvm.NavigateOutcomes();
        }

        #endregion
    }
}
