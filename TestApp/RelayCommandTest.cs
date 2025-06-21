using NucleusWPF.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Annotations;

namespace TestApp
{
    [TestClass]
    public class RelayCommandTest
    {
        [TestMethod]
        public void CanExecuteTest()
        {
            var canExecute = true;
            var command = new RelayCommand(() => { }, () => canExecute);
            var command2 = new RelayCommand(() => { });
            Assert.IsTrue(command.CanExecute(null));
            Assert.IsTrue(command2.CanExecute(null), "RelayCommand without canExecute did not default to true");
            canExecute = false;
            Assert.IsFalse(command.CanExecute(null), "CanExecute returned true when should have been false");
        }

        [TestMethod]
        public void ExecuteTest()
        {
            var executed = false;
            var canExecute = true;
            var command = new RelayCommand(() => executed = true, () => canExecute);
            command.Execute(null);
            Assert.IsTrue(executed, "Command did not execute when it should have");
            executed = false;
            canExecute = false;
            command.Execute(null);
            Assert.IsFalse(executed, "Command executed when it should not have");
        }

        [TestMethod]
        public void RaiseCanExecuteChangedTest()
        {
            bool eventRaised = false;
            var command = new RelayCommand(() => { });
            command.CanExecuteChanged += (s, e) => eventRaised = true;
            command.RaiseCanExecuteChanged();
            Assert.IsTrue(eventRaised, "RaiseCanExecuteChanged did not raise CanExecuteChanged event");
        }
    }
}
