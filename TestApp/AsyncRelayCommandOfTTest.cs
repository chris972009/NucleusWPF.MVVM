using NucleusWPF.MVVM;

namespace TestApp
{
    [TestClass]
    public class AsyncRelayCommandOfTTest
    {
        [TestMethod]
        public void CanExecuteTest()
        {
            bool canExecute = true;
            Func<bool, Task> t = (_) => Task.Run(() => { });
            var command1 = new AsyncRelayCommand<bool>(t, (bool canExecute) => canExecute);
            var command2 = new AsyncRelayCommand<bool>(t, () => canExecute);
            var command3 = new AsyncRelayCommand<bool>(t);

            Assert.IsTrue(command1.CanExecute(true));
            Assert.IsTrue(command2.CanExecute(null), "Parameterless CanExecute returned false");
            Assert.IsTrue(command3.CanExecute(null), "CanExecute did not default to true");

            canExecute = false;
            Assert.IsFalse(command1.CanExecute(false), "CanExecute returned true whith true input");
            Assert.IsFalse(command2.CanExecute(null), "Parameterless CanExecute returned true when it should be false");
        }

        [TestMethod]
        public async Task ExecuteAsyncTest()
        {
            bool executedFlag = false;
            bool canExecute = true;
            Func<bool, Task> execute = (bool value) => Task.Run(() => executedFlag = value);
            var command = new AsyncRelayCommand<bool>(execute, () => canExecute);
            await command.ExecuteAsync(true);
            Assert.IsTrue(executedFlag);
            canExecute = false;
            await command.ExecuteAsync(false);
            Assert.IsTrue(executedFlag, "Command executed when it should not have");
        }

        [TestMethod]
        public void RaiseCanExecuteChangedTest()
        {
            bool executed = false;
            var command = new AsyncRelayCommand<bool>((_) => Task.Run(() => { }));
            command.CanExecuteChanged += (sender, args) => executed = true;
            command.RaiseCanExecuteChanged();
            Assert.IsTrue(executed, "CanExecuteChanged event was not raised when expected.");
        }
    }
}
