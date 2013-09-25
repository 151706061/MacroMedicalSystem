#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Macro.Web.Client.Silverlight.Utilities;

namespace Macro.Web.Client.Silverlight
{

	/// <summary>
	/// Defines the logging level for calls to one of the <b>Platform.Log</b> methods.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Debug log level.
		/// </summary>
		Debug,
		/// <summary>
		/// Info log level.
		/// </summary>
		Info,
		/// <summary>
		/// Warning log level.
		/// </summary>
		Warn,
		/// <summary>
		/// Error log level.
		/// </summary>
		Error,
		/// <summary>
		/// Fatal log level.
		/// </summary>
		Fatal
	}


	/// <summary>
	/// A collection of useful utility functions.
	/// </summary>
	public static class Platform
	{
        static Platform()
        {        
            IsErrorEnabled = true;
            IsFatalEnabled = true;
        }

        /// <summary>
        /// Delegate definition for receiving log lines
        /// </summary>
        /// <param name="msg"></param>
        public delegate void LogDelegate(String msg);

        /// <summary>
        /// Delegate
        /// </summary>
        public static LogDelegate Logger;

        /// <summary>
        /// Gets the current time from an extension of <see cref="TimeProviderExtensionPoint"/>, if one exists.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The time returned may differ from the current time on the local machine, because the provider may choose
        /// to obtain the time from another source (i.e. a server).
		/// </para>
		/// <para>
        /// This method is thread-safe.
		/// </para>
        /// </remarks>
        public static DateTime Time
        {
            get { return DateTime.Now; }
        }

        public static bool IsDebugEnabled { get; set; }
        public static bool IsInfoEnabled { get; set; }
        public static bool IsWarnEnabled { get; set; }
        public static bool IsErrorEnabled { get; set; }
        public static bool IsFatalEnabled { get; set; }

		/// <summary>
		/// Determines if the specified <see cref="LogLevel"/> is enabled.
		/// </summary>
		/// <param name="category">The logging level to check.</param>
		/// <returns>true if the <see cref="LogLevel"/> is enabled, or else false.</returns>
		public static bool IsLogLevelEnabled(LogLevel category)
		{
			switch (category)
			{
				case LogLevel.Debug:
					return IsDebugEnabled;
				case LogLevel.Info:
					return IsInfoEnabled;
				case LogLevel.Warn:
					return IsWarnEnabled;
				case LogLevel.Error:
					return IsErrorEnabled;
				case LogLevel.Fatal:
					return IsFatalEnabled;
			}
			return false;
		}

        /// <summary>
        /// Logs the specified message at the specified <see cref="LogLevel"/>.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <param name="category">The logging level.</param>
		/// <param name="message">The message to be logged.</param>
		public static void Log(LogLevel category, object message)
        {
			// Just return without formatting if the log level isn't enabled
			if (!IsLogLevelEnabled(category)) return;

            var ex = message as Exception;
            if (ex != null)
            {
                switch (category)
                {
                    case LogLevel.Debug:
                        LogToDelegate("Debug", SR.ExceptionThrown, ex);
                        break;
                    case LogLevel.Info:
                        LogToDelegate("Info", SR.ExceptionThrown, ex);
                        break;
                    case LogLevel.Warn:
                        LogToDelegate("Warn", SR.ExceptionThrown, ex);
                        break;
                    case LogLevel.Error:
                        LogToDelegate("Error", SR.ExceptionThrown, ex);
                        break;
                    case LogLevel.Fatal:
                        LogToDelegate("Fatal", SR.ExceptionThrown, ex);
                        break;
                }
            }
            else
            {
                switch (category)
                {
                    case LogLevel.Debug:
                        LogToDelegate("Debug", message.ToString());
                        break;
                    case LogLevel.Info:
                        LogToDelegate("Info", message.ToString());
                        break;
                    case LogLevel.Warn:
                        LogToDelegate("Warn", message.ToString());
                        break;
                    case LogLevel.Error:
                        LogToDelegate("Error", message.ToString());
                        break;
                    case LogLevel.Fatal:
                        LogToDelegate("Fatal", message.ToString());
                        break;
                }
            }
        }

        /// <summary>
        /// Logs the specified message at the specified <see cref="LogLevel"/>.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <param name="category">The log level.</param>
        /// <param name="message">Format message, as used with <see cref="System.Text.StringBuilder"/>.</param>
        /// <param name="args">Optional arguments used with <paramref name="message"/>.</param>
        public static void Log(LogLevel category,String message, params object[] args)
        {
			// Just return without formatting if the log level isn't enabled
			if (!IsLogLevelEnabled(category)) return;

			var sb = new StringBuilder();

			if (args == null || args.Length == 0)
				sb.Append(message);
			else
				sb.AppendFormat(message, args);

            switch (category)
            {
                case LogLevel.Debug:
                    LogToDelegate("Debug", sb.ToString());
                    break;
                case LogLevel.Info:
                    LogToDelegate("Info", sb.ToString());
                    break;
                case LogLevel.Warn:
                    LogToDelegate("Warn", sb.ToString());
                    break;
                case LogLevel.Error:
                    LogToDelegate("Error", sb.ToString());
                    break;
                case LogLevel.Fatal:
                    LogToDelegate("Fatal", sb.ToString());
                    break;
            }
        }


        /// <summary>
        /// Logs the specified exception at the specified <see cref="LogLevel"/>.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <param name="ex">The exception to log.</param>
        /// <param name="category">The log level.</param>
        /// <param name="message">Format message, as used with <see cref="System.Text.StringBuilder"/>.</param>
        /// <param name="args">Optional arguments used with <paramref name="message"/>.</param>
        public static void Log(LogLevel category, Exception ex, String message, params object[] args)
        {
			// Just return without formatting if the log level isn't enabled
			if (!IsLogLevelEnabled(category)) return;

			StringBuilder sb = new StringBuilder();
            sb.AppendLine(SR.ExceptionThrown);
            sb.AppendLine();

			if (args == null || args.Length == 0)
				sb.Append(message);
			else
				sb.AppendFormat(message, args);
            
            switch (category)
            {
                case LogLevel.Debug:
                    LogToDelegate("Debug", sb.ToString(), ex);
                    break;
                case LogLevel.Info:
                    LogToDelegate("Info", sb.ToString(), ex);
                    break;
                case LogLevel.Warn:
                    LogToDelegate("Warn", sb.ToString(), ex);
                    break;
                case LogLevel.Error:
                    LogToDelegate("Error", sb.ToString(), ex);
                    break;
                case LogLevel.Fatal:
                    LogToDelegate("Fatal", sb.ToString(), ex);
                    break;
            }
        }

        private static void LogToDelegate(string level, string message)
        {
            if (Logger != null)
            {
                DateTime now = DateTime.Now;

                string threadName = Thread.CurrentThread.Name;

                string logLine = string.IsNullOrEmpty(threadName)
                                     ? string.Format("{0} [{1}] {2} - {3}", now.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId,
                                                      level, message)
                                     : string.Format("{0} [{1} [{2}]] {3} - {4}", now.ToString("yyyy-MM-dd HH:mm:ss.fff"), threadName, Thread.CurrentThread.ManagedThreadId,
                                     level,message);
                UIThread.Execute(() => Logger(logLine));
            }
        }

        private static void LogToDelegate(string level, string message, Exception e)
        {
            var sb = new StringBuilder();

            sb.AppendFormat(message);
            sb.AppendLine();
            sb.AppendFormat("Exception: {0} ", e.Message);
            sb.AppendLine();
            sb.AppendLine("Stack Trace:");
            sb.AppendLine(e.StackTrace);
            if (e.InnerException != null)
            {
                sb.AppendFormat("InnerException: {0} ", e.InnerException.Message);
                sb.AppendLine();
                sb.AppendLine("InnerException Stack Trace:");
                sb.AppendLine(e.InnerException.StackTrace);
                if (e.InnerException.InnerException != null)
                {
                    sb.AppendFormat("InnerInnerException: {0} ", e.InnerException.InnerException.Message);
                    sb.AppendLine();
                    sb.AppendLine("InnerInnerException Stack Trace:");
                    sb.AppendLine(e.InnerException.InnerException.StackTrace);
                }
            }
            LogToDelegate(level,sb.ToString());
        }

		/// <summary>
		/// Checks if a string is empty.
		/// </summary>
		/// <param name="variable">The string to check.</param>
		/// <param name="variableName">The variable name of the string to checked.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variable"/> or or <paramref name="variableName"/>
		/// is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="variable"/> is zero length.</exception>
		public static void CheckForEmptyString(string variable, string variableName)
		{
			CheckForNullReference(variable, variableName);
			CheckForNullReference(variableName, "variableName");

			if (variable.Length == 0)
				throw new ArgumentException(String.Format(SR.ExceptionEmptyString, variableName));
		}

		/// <summary>
		/// Checks if an object reference is null.
		/// </summary>
		/// <param name="variable">The object reference to check.</param>
		/// <param name="variableName">The variable name of the object reference to check.</param>
		/// <remarks>Use for checking if an input argument is <b>null</b>.  To check if a member variable
		/// is <b>null</b> (i.e., to see if an object is in a valid state), use <b>CheckMemberIsSet</b> instead.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variable"/> or <paramref name="variableName"/>
		/// is <b>null</b>.</exception>
		public static void CheckForNullReference(object variable, string variableName)
		{
			if (variableName == null)
				throw new ArgumentNullException("variableName");

			if (null == variable)
				throw new ArgumentNullException(variableName);
		}

		/// <summary>
		/// Checks if an object is of the expected type.
		/// </summary>
		/// <param name="variable">The object to check.</param>
		/// <param name="type">The variable name of the object to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variable"/> or <paramref name="type"/>
		/// is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> is not the expected type.</exception>
		public static void CheckExpectedType(object variable, Type type)
		{
			CheckForNullReference(variable, "variable");
			CheckForNullReference(type, "type");

			if (!type.IsAssignableFrom(variable.GetType()))
				throw new ArgumentException(String.Format(SR.ExceptionExpectedType, type.FullName));
		}

		/// <summary>
		/// Checks if a value is positive.
		/// </summary>
		/// <param name="n">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="n"/> &lt;= 0.</exception>
		public static void CheckPositive(int n, string variableName)
		{
			CheckForNullReference(variableName, "variableName");

			if (n <= 0)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}


        /// <summary>
        /// Checks if a value is true.
        /// </summary>
        /// <param name="testTrueCondition">The value to check.</param>
        /// <param name="conditionName">The name of the condition to check.</param>
        /// <exception cref="ArgumentNullException"><paramref name="conditionName"/> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException"><paramref name="testTrueCondition"/> is  <b>false</b>.</exception>
        public static void CheckTrue(bool testTrueCondition, string conditionName)
        {
            Platform.CheckForNullReference(conditionName, "conditionName");

            if (testTrueCondition != true)
                throw new ArgumentException(String.Format(SR.ExceptionConditionIsNotMet, conditionName));
        }


        /// <summary>
        /// Checks if a value is false.
        /// </summary>
        /// <param name="testFalseCondition">The value to check.</param>
        /// <param name="conditionName">The name of the condition to check.</param>
        /// <exception cref="ArgumentNullException"><paramref name="conditionName"/> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException"><paramref name="testFalseCondition"/> is  <b>true</b>.</exception>
        public static void CheckFalse(bool testFalseCondition, string conditionName)
        {
            Platform.CheckForNullReference(conditionName, "conditionName");

            if (testFalseCondition != false)
				throw new ArgumentException(String.Format(SR.ExceptionConditionIsNotMet, conditionName));
        }

		/// <summary>
		/// Checks if a value is positive.
		/// </summary>
		/// <param name="x">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="x"/> &lt;= 0.</exception>
		public static void CheckPositive(float x, string variableName)
		{
			Platform.CheckForNullReference(variableName, "variableName");

			if (x <= 0.0f)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}

		/// <summary>
		/// Checks if a value is positive.
		/// </summary>
		/// <param name="x">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="x"/> &lt;= 0.</exception>
		public static void CheckPositive(double x, string variableName)
		{
			Platform.CheckForNullReference(variableName, "variableName");

			if (x <= 0.0d)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}

		/// <summary>
		/// Checks if a value is non-negative.
		/// </summary>
		/// <param name="n">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="n"/> &lt; 0.</exception>
		public static void CheckNonNegative(int n, string variableName)
		{
			Platform.CheckForNullReference(variableName, "variableName");

			if (n < 0)
				throw new ArgumentException(SR.ExceptionArgumentNegative, variableName);
		}

		/// <summary>
		/// Checks if a value is within a specified range.
		/// </summary>
		/// <param name="argumentValue">Value to be checked.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="variableName">Variable name of value to be checked.</param>
		/// <remarks>Checks if <paramref name="min"/> &lt;= <paramref name="argumentValue"/> &lt;= <paramref name="max"/></remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="argumentValue"/> is not within the
		/// specified range.</exception>
		public static void CheckArgumentRange(int argumentValue, int min, int max, string variableName)
		{
			Platform.CheckForNullReference(variableName, "variableName");

			if (argumentValue < min || argumentValue > max)
				throw new ArgumentOutOfRangeException(String.Format(SR.ExceptionArgumentOutOfRange, argumentValue, min, max, variableName));
		}

		/// <summary>
		/// Checks if an index is within a specified range.
		/// </summary>
		/// <param name="index">Index to be checked</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="obj">Object being indexed.</param>
		/// <remarks>Checks if <paramref name="min"/> &lt;= <paramref name="index"/> &lt;= <paramref name="max"/>.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="obj"/> is <b>null</b>.</exception>
		/// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is not within the
		/// specified range.</exception>
		public static void CheckIndexRange(int index, int min, int max, object obj)
		{
			Platform.CheckForNullReference(obj, "obj");

			if (index < min || index > max)
				throw new IndexOutOfRangeException(String.Format(SR.ExceptionIndexOutOfRange, index, min, max, obj.GetType().Name));
		}

		/// <summary>
		/// Checks if a field or property is null.
		/// </summary>
		/// <param name="variable">Field or property to be checked.</param>
		/// <param name="variableName">Name of field or property to be checked.</param>
		/// <remarks>Use this method in your classes to verify that the object
		/// is not in an invalid state by checking that various fields and/or properties
		/// have been set, i.e., are not null.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="variable"/> is <b>null</b>.</exception>
		public static void CheckMemberIsSet(object variable, string variableName)
		{
			Platform.CheckForNullReference(variableName, "variableName");

			if (variable == null)
				throw new InvalidOperationException(String.Format(SR.ExceptionMemberNotSet, variableName));
		}

		/// <summary>
		/// Checks if a field or property is null.
		/// </summary>
		/// <param name="variable">Field or property to be checked.</param>
		/// <param name="variableName">Name of field or property to be checked.</param>
		/// <param name="detailedMessage">A more detailed and informative message describing
		/// why the object is in an invalid state.</param>
		/// <remarks>Use this method in your classes to verify that the object
		/// is not in an invalid state by checking that various fields and/or properties
		/// have been set, i.e., are not null.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="variable"/> is <b>null</b>.</exception>
		public static void CheckMemberIsSet(object variable, string variableName, string detailedMessage)
		{
			Platform.CheckForNullReference(variableName, "variableName");
			Platform.CheckForNullReference(detailedMessage, "detailedMessage");

			if (variable == null)
				throw new InvalidOperationException(String.Format(SR.ExceptionMemberNotSetVerbose, variableName, detailedMessage));
		}
    }
}
