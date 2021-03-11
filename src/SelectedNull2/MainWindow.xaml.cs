using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SelectedNull2 {
  public class Command : ICommand {
    private readonly MainWindow _mainWindow;

    public Command(MainWindow mainWindow) {
      _mainWindow = mainWindow;
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object _) => true;
    public void Execute(object _) {
      _mainWindow.UserModel = new UserModel();
      _mainWindow.UserModel.Users.Add(new User { Id = 1 });
      _mainWindow.UserModel.Users.Add(new User { Id = 2 });
      _mainWindow.UserModel.Users.Add(new User { Id = 3 });
      _mainWindow.UserModel.Users.Add(new User { Id = 4 });
      _mainWindow.UserModel.Users.Add(new User { Id = 5 });
    }
  }

  public class User {
    public int Id { get; set; }
  }

  public class UserModel {

    public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

    public User SelectedUser {
      get => null;
      set { }
    }
  }

  public partial class MainWindow : Window, INotifyPropertyChanged {

    public ICommand ShowUsers { get; }

    private UserModel _userModel;
    public UserModel UserModel {
      get => _userModel;
      set {
        _userModel = value;
        OnPropertyChanged();
      }
    }

    public MainWindow() {
      DataContext = this;
      ShowUsers = new Command(this);
      InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string name = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}