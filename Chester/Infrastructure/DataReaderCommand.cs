using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Chester.Infrastructure
{
    public class DataReaderCommand : IDisposable
    {
        #region Fields
        bool _isDisposed;
        #endregion

        #region Properties
        public IDbCommand Command { get; private set; }

        public IDataReader DataReader { get; private set; }
        #endregion

        #region Constructors
        public DataReaderCommand(IDbCommand cmd, CommandBehavior cmdBehavior)
        {
            Command = cmd;
            DataReader = cmd.ExecuteReader(cmdBehavior);
        }
        #endregion

        #region Destructor
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (DataReader != null)
                    {
                        Close();

                        DataReader.Dispose();
                        DataReader = null;
                    }

                    if (Command != null)
                    {
                        Command.Dispose();
                        Command = null;
                    }
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Methods
        public void Close()
        {
            if (!(DataReader?.IsClosed ?? true))
                DataReader.Close();
        }
        #endregion
    }
}
