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
    public enum AutocompleteTerm
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
                AutocompleteTerm.None => () => null,
                AutocompleteTerm.FieldName => () => column.FieldName,
                AutocompleteTerm.Defeat => () => Guid.NewGuid().ToString(),
                AutocompleteTerm.FullName => () => "name",
                AutocompleteTerm.HonorificPrefix => () => "honorific-prefix",
                AutocompleteTerm.GivenName => () => "given-name",
                AutocompleteTerm.AdditionalName => () => "additional-name",
                AutocompleteTerm.FamilyName => () => "family-name",
                AutocompleteTerm.HonorificSuffix => () => "honorific-suffix",
                AutocompleteTerm.NickName => () => "nickname",
                AutocompleteTerm.Email => () => "email",
                AutocompleteTerm.UserName => () => "username",
                AutocompleteTerm.NewPassword => () => "new-password",
                AutocompleteTerm.CurrentPassword => () => "current-password",
                AutocompleteTerm.OneTimeCode => () => "one-time-code",
                AutocompleteTerm.OrgTitle => () => "organization-title",
                AutocompleteTerm.Org => () => "organization",
                AutocompleteTerm.StreetAddress => () => "street-address",
                AutocompleteTerm.AddressLine1 => () => "address-line1",
                AutocompleteTerm.AddressLine2 => () => "address-line2",
                AutocompleteTerm.AddressLine3 => () => "address-line3",
                AutocompleteTerm.AddressLevel1 => () => "address-level1",
                AutocompleteTerm.AddressLevel2 => () => "address-level2",
                AutocompleteTerm.AddressLevel3 => () => "address-level3",
                AutocompleteTerm.AddressLevel4 => () => "address-level4",
                AutocompleteTerm.Country => () => "country",
                AutocompleteTerm.CountryName => () => "country-name",
                AutocompleteTerm.PostalCode => () => "postal-code",
                AutocompleteTerm.CCName => () => "cc-name",
                AutocompleteTerm.CCGivenName => () => "cc-given-name",
                AutocompleteTerm.CCAdditionalName => () => "cc-additional-name",
                AutocompleteTerm.CCFamilyName => () => "cc-family-name",
                AutocompleteTerm.CCNumber => () => "cc-number",
                AutocompleteTerm.CCExp => () => "cc-exp",
                AutocompleteTerm.CCExpMonth => () => "cc-exp-month",
                AutocompleteTerm.CCExpYear => () => "cc-exp-year",
                AutocompleteTerm.CCCsc => () => "cc-csc",
                AutocompleteTerm.CCType => () => "cc-type",
                AutocompleteTerm.TransactionCurrency => () => "transaction-currency",
                AutocompleteTerm.TransactionAmount => () => "transaction-amount",
                AutocompleteTerm.Language => () => "language",
                AutocompleteTerm.BDay => () => "bday",
                AutocompleteTerm.BDayDay => () => "bday-day",
                AutocompleteTerm.BDayMonth => () => "bday-month",
                AutocompleteTerm.BDayYear => () => "bday-year",
                AutocompleteTerm.Sex => () => "sex",
                AutocompleteTerm.Tel => () => "tel",
                AutocompleteTerm.TelCountryCode => () => "tel-country-code",
                AutocompleteTerm.TelNational => () => "tel-national",
                AutocompleteTerm.TelAreaCode => () => "tel-area-code",
                AutocompleteTerm.TelLocal => () => "tel-local",
                AutocompleteTerm.TelExtension => () => "tel-extension",
                AutocompleteTerm.Impp => () => "impp",
                AutocompleteTerm.Url => () => "url",
                AutocompleteTerm.Photo => () => "photo",
                _ => () => "on"
            };
    }
}
