using NucleusWPF.MVVM;

namespace TestApp
{
    [TestClass]
    public class AsyncRelayCommandTest
    {
        [TestMethod]
        public void CanExecuteTest()
        {
            bool canExecute = true;
            var command = new AsyncRelayCommand(() => Task.Run(() => { }), () => canExecute);
            Assert.IsTrue(command.CanExecute(null));
            canExecute = false;
            Assert.IsFalse(command.CanExecute(null));

            command = new AsyncRelayCommand(() => Task.Run(() => { }));
            Assert.IsTrue(command.CanExecute(null), "Command without canExecute parameter returned false");
        }

        [TestMethod]
        public async Task ExecuteTest()
        {
            bool executed = false;
            bool canExecute = true;
            var command = new AsyncRelayCommand(() => Task.Run(() => executed = true), () => canExecute);
            await command.ExecuteAsync();
            Assert.IsTrue(executed);
            executed = false;
            canExecute = false;
            await command.ExecuteAsync();
            Assert.IsFalse(executed, "Command executed when it should not have been allowed to execute.");
        }

        [TestMethod]
        public void RaiseCanExecuteChanged()
        {
            bool executed = false;
            var command = new AsyncRelayCommand(() => Task.Run(() => { }));
            command.CanExecuteChanged += (s, e) => executed = true;
            command.RaiseCanExecuteChanged();
            Assert.IsTrue(executed, "RaiseCanExecuteChanged did not trigger CanExecuteChanged event.");
        }
    }
}
