using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SelectedNull2 {
  public class Command : ICommand {
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object _) => true;
    public void Execute(object obj) {
      MainWindow.Instance.UserModel = new UserModel();
      MainWindow.Instance.UserModel.Users.Add(new User { Id = 1 });
      MainWindow.Instance.UserModel.Users.Add(new User { Id = 2 });
      MainWindow.Instance.UserModel.Users.Add(new User { Id = 3 });
      MainWindow.Instance.UserModel.Users.Add(new User { Id = 4 });
      MainWindow.Instance.UserModel.Users.Add(new User { Id = 5 });
    }
  }

  public class User {
    public int Id { get; set; }
  }

  public class UserModel {
    public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();
    public User SelectedUser { get; set; }
  }

  public partial class MainWindow : Window, INotifyPropertyChanged {

    public ICommand ShowUsers { get; } = new Command();


    public event PropertyChangedEventHandler PropertyChanged;

    private UserModel _userModel;
    public UserModel UserModel {
      get => _userModel;
      set {
        _userModel = value;
        OnPropertyChanged();
      }
    }
    private void OnPropertyChanged([CallerMemberName] string name = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public static MainWindow Instance { get; set; }

    public MainWindow() {
      DataContext = this;
      Instance = this;
      InitializeComponent();
    }
  }
}