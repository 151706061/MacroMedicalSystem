#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.Enterprise.Common.Admin.AuthorityGroupAdmin;
using Macro.ImageServer.Services.Common;
using Macro.Web.Enterprise.Admin;

namespace Macro.ImageServer.Web.Common.Data.DataSource
{
	public class TokenDataSource : BaseDataSource
	{
        #region Private Members

		private int _resultCount;
        #endregion Private Members

        #region Public Members
        public delegate void TokenFoundSetDelegate(IList<TokenRowData> list);
        public TokenFoundSetDelegate TokenFoundSet;
        #endregion Public Members

        #region Properties
        public int ResultCount
        {
            get { return _resultCount; }
            set { _resultCount = value; }
        }
        #endregion

        #region Private Methods

        private IList<TokenRowData> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
        {
            Array tokenRowData = null;
            Array tokenRowDataRange = Array.CreateInstance(typeof(TokenRowData), maximumRows);

            resultCount = 0;

            if (maximumRows == 0) return new List<TokenRowData>();

            using(AuthorityManagement service = new AuthorityManagement())
            {
                IList<AuthorityTokenSummary> tokens = service.ListAuthorityTokens();
                List<TokenRowData> tokenRows = CollectionUtils.Map<AuthorityTokenSummary, TokenRowData>(
                    tokens, delegate(AuthorityTokenSummary token)
                           {
                               TokenRowData row = new TokenRowData(token);
                               return row;
                           });

                tokenRowData = CollectionUtils.ToArray(tokenRows);

                int copyLength = adjustCopyLength(startRowIndex, maximumRows, tokenRowData.Length);

                Array.Copy(tokenRowData, startRowIndex, tokenRowDataRange, 0, copyLength);

                if(copyLength < tokenRowDataRange.Length)
                {
                    tokenRowDataRange = resizeArray(tokenRowDataRange, copyLength);
                }
            };

            if (tokenRowData != null)
            {
                resultCount = tokenRowData.Length;
            }

            return CollectionUtils.Cast<TokenRowData>(tokenRowDataRange);
        }

        #endregion Private Methods

        #region Public Methods
        public IEnumerable<TokenRowData> Select(int startRowIndex, int maximumRows)
        {
            IList<TokenRowData> _list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

            if (TokenFoundSet != null)
                TokenFoundSet(_list);

            return _list;

        }

        public int SelectCount()
        {
            if (ResultCount != 0) return ResultCount;

            // Ignore the search results
            InternalSelect(0, 1, out _resultCount);

            return ResultCount;
        }

        #endregion Public Methods
    }

    [Serializable]
    public class TokenRowData
    {
        private string _name;
        private string _description;

        public TokenRowData(AuthorityTokenSummary token)
        {
            Name = token.Name;
            Description = token.Description;
        }

        public TokenRowData()
        {
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }

        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        } 
    }
}