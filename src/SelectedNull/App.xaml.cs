using System;
using System.Windows;
using SubModelSeqInSubModelOpt.Core;

namespace SubModelSeqInSubModelOpt {
  public partial class App : Application {
    public App() {
      this.Activated += StartElmish;
    }

    private void StartElmish(object sender, EventArgs e) {
      this.Activated -= StartElmish;
      MainApp.main(MainWindow);
    }
  }
}