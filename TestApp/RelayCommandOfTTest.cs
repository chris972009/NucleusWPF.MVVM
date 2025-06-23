using NucleusWPF.MVVM;

namespace TestApp
{
    [TestClass]
    public class RelayCommandOfTTest
    {
        [TestMethod]
        public void CanExecuteTest()
        {
            bool canExecute = true;
            var command = new RelayCommand<bool>((_) => { }, (bool b) => b);
            var command2 = new RelayCommand<bool>((_) => { }, () => canExecute);
            var command3 = new RelayCommand<bool>((_) => { });

            Assert.IsTrue(command.CanExecute(true));
            Assert.IsTrue(command2.CanExecute(null), "Parameterless CanExecute returned false");
            Assert.IsTrue(command3.CanExecute(null), "RelayCommand<T> CanExecute did not default to true");

            canExecute = false;
            Assert.IsFalse(command.CanExecute(false));
            Assert.IsFalse(command2.CanExecute(null), "Parameterless CanExecute returned true when it should be false");
        }

        [TestMethod]
        public void ExecuteTest()
        {
            var executed = false;
            bool canExecute = true;
            var command = new RelayCommand<bool>((bool b) => executed = b, () => canExecute);

            command.Execute(true);
            Assert.IsTrue(executed);

            canExecute = false;
            command.Execute(false);
            Assert.IsTrue(executed, "Command executed when it should not have been allowed to execute.");
        }

        [TestMethod]
        public void RaiseCanExecuteChangedTest()
        {
            var eventRaised = false;
            var commnand = new RelayCommand<bool>((_) => { });
            commnand.CanExecuteChanged += (s, e) => eventRaised = true;
            commnand.RaiseCanExecuteChanged();
            Assert.IsTrue(eventRaised, "RaiseCanExecuteChanged did not raise the CanExecuteChanged event.");
        }
    }
}
