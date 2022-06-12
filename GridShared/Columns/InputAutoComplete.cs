using System;
using System.Collections.Generic;
using System.Text;

namespace GridShared.Columns
{
    /// <summary>
    /// <para>
    /// Values are mostly from Mozilla developer docs.
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Attributes/autocomplete"/>
    /// <seealso href="https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#autofill"/>
    /// </para>
    /// </summary>
    public enum AutoCompleteTerm
    {
        /// <summary>
        /// Uses the default autocomplete as if it was never set.
        /// </summary>
        None,
        /// <summary>
        /// Uses the <c>FieldName</c> to set the term.
        /// <see cref="IColumn.FieldName"/>
        /// </summary>
        FieldName,
        /// <summary>
        /// Uses the value from the <c>CustomAutocomplete</c> member.
        /// <see cref="IColumn.CustomAutoComplete"/>
        /// </summary>
        Custom,
        /// <summary>
        /// Uses a random GUID to prevent matching.
        /// </summary>
        Defeat,
        FullName,
        HonorificPrefix,
        GivenName,
        AdditionalName,
        FamilyName,
        HonorificSuffix,
        NickName,
        Email,
        UserName,
        NewPassword,
        CurrentPassword,
        OneTimeCode,
        OrgTitle,
        Org,
        StreetAddress,
        AddressLine1,
        AddressLine2,
        AddressLine3,
        AddressLevel1,
        AddressLevel2,
        AddressLevel3,
        AddressLevel4,
        Country,
        CountryName,
        PostalCode,
        CCName,
        CCGivenName,
        CCAdditionalName,
        CCFamilyName,
        CCNumber,
        CCExp,
        CCExpMonth,
        CCExpYear,
        CCCsc,
        CCType,
        TransactionCurrency,
        TransactionAmount,
        Language,
        BDay,
        BDayDay,
        BDayMonth,
        BDayYear,
        Sex,
        Tel,
        TelCountryCode,
        TelNational,
        TelAreaCode,
        TelLocal,
        TelExtension,
        Impp,
        Url,
        Photo
    }

    public static class InputAutocompleteExtensions
    {
        public static Func<string> ToAutocompleteFunc(this IColumn column) =>
            column.AutoCompleteTaxonomy switch
            {
                AutoCompleteTerm.None => () => null,
                AutoCompleteTerm.FieldName => () => column.FieldName,
                AutoCompleteTerm.Defeat => () => Guid.NewGuid().ToString(),
                AutoCompleteTerm.FullName => () => "name",
                AutoCompleteTerm.HonorificPrefix => () => "honorific-prefix",
                AutoCompleteTerm.GivenName => () => "given-name",
                AutoCompleteTerm.AdditionalName => () => "additional-name",
                AutoCompleteTerm.FamilyName => () => "family-name",
                AutoCompleteTerm.HonorificSuffix => () => "honorific-suffix",
                AutoCompleteTerm.NickName => () => "nickname",
                AutoCompleteTerm.Email => () => "email",
                AutoCompleteTerm.UserName => () => "username",
                AutoCompleteTerm.NewPassword => () => "new-password",
                AutoCompleteTerm.CurrentPassword => () => "current-password",
                AutoCompleteTerm.OneTimeCode => () => "one-time-code",
                AutoCompleteTerm.OrgTitle => () => "organization-title",
                AutoCompleteTerm.Org => () => "organization",
                AutoCompleteTerm.StreetAddress => () => "street-address",
                AutoCompleteTerm.AddressLine1 => () => "address-line1",
                AutoCompleteTerm.AddressLine2 => () => "address-line2",
                AutoCompleteTerm.AddressLine3 => () => "address-line3",
                AutoCompleteTerm.AddressLevel1 => () => "address-level1",
                AutoCompleteTerm.AddressLevel2 => () => "address-level2",
                AutoCompleteTerm.AddressLevel3 => () => "address-level3",
                AutoCompleteTerm.AddressLevel4 => () => "address-level4",
                AutoCompleteTerm.Country => () => "country",
                AutoCompleteTerm.CountryName => () => "country-name",
                AutoCompleteTerm.PostalCode => () => "postal-code",
                AutoCompleteTerm.CCName => () => "cc-name",
                AutoCompleteTerm.CCGivenName => () => "cc-given-name",
                AutoCompleteTerm.CCAdditionalName => () => "cc-additional-name",
                AutoCompleteTerm.CCFamilyName => () => "cc-family-name",
                AutoCompleteTerm.CCNumber => () => "cc-number",
                AutoCompleteTerm.CCExp => () => "cc-exp",
                AutoCompleteTerm.CCExpMonth => () => "cc-exp-month",
                AutoCompleteTerm.CCExpYear => () => "cc-exp-year",
                AutoCompleteTerm.CCCsc => () => "cc-csc",
                AutoCompleteTerm.CCType => () => "cc-type",
                AutoCompleteTerm.TransactionCurrency => () => "transaction-currency",
                AutoCompleteTerm.TransactionAmount => () => "transaction-amount",
                AutoCompleteTerm.Language => () => "language",
                AutoCompleteTerm.BDay => () => "bday",
                AutoCompleteTerm.BDayDay => () => "bday-day",
                AutoCompleteTerm.BDayMonth => () => "bday-month",
                AutoCompleteTerm.BDayYear => () => "bday-year",
                AutoCompleteTerm.Sex => () => "sex",
                AutoCompleteTerm.Tel => () => "tel",
                AutoCompleteTerm.TelCountryCode => () => "tel-country-code",
                AutoCompleteTerm.TelNational => () => "tel-national",
                AutoCompleteTerm.TelAreaCode => () => "tel-area-code",
                AutoCompleteTerm.TelLocal => () => "tel-local",
                AutoCompleteTerm.TelExtension => () => "tel-extension",
                AutoCompleteTerm.Impp => () => "impp",
                AutoCompleteTerm.Url => () => "url",
                AutoCompleteTerm.Photo => () => "photo",
                _ => () => "on"
            };
    }
}
