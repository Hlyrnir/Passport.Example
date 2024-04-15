﻿using Domain.Interface.Authorization;
using Domain.Interface.Transfer;

namespace Domain.Aggregate.Authorization.PassportHolder
{
    public sealed class PassportHolder : IPassportHolder
    {
        private string sConcurrencyStamp;
        private string sCultureName;
        private string sEmailAddress;
        private bool bEmailAddressIsConfirmed;
        private string sFirstName;
        private Guid guId;
        private string sLastName;
        private string sPhoneNumber;
        private bool bPhoneNumberIsConfirmed;
        private string sSecurityStamp;

        private PassportHolder(
            string sConcurrencyStamp,
            string sCultureName,
            string sEmailAddress,
            bool bEmailAddressIsConfirmed,
            string sFirstName,
            Guid guId,
            string sLastName,
            string sPhoneNumber,
            bool bPhoneNumberIsConfirmed,
            string sSecurityStamp)
        {
            this.sConcurrencyStamp = sConcurrencyStamp;
            this.sCultureName = sCultureName;
            this.sEmailAddress = sEmailAddress;
            this.bEmailAddressIsConfirmed = bEmailAddressIsConfirmed;
            this.sFirstName = sFirstName;
            this.guId = guId;
            this.sLastName = sLastName;
            this.sPhoneNumber = sPhoneNumber;
            this.bPhoneNumberIsConfirmed = bPhoneNumberIsConfirmed;
            this.sSecurityStamp = sSecurityStamp;
        }

        public string ConcurrencyStamp { get => sConcurrencyStamp; }
        public string CultureName { get => sCultureName; }
        public string EmailAddress { get => sEmailAddress; }
        public bool EmailAddressIsConfirmed { get => bEmailAddressIsConfirmed; }
        public string FirstName { get => sFirstName; set => sFirstName = value; }
        public Guid Id { get => guId; }
        public string LastName { get => sLastName; set => sLastName = value; }
        public string PhoneNumber { get => sPhoneNumber; }
        public bool PhoneNumberIsConfirmed { get => bPhoneNumberIsConfirmed; }
        public string SecurityStamp { get => sSecurityStamp; }

        public bool TryChangeCultureName(string sCultureName)
        {
            if (string.IsNullOrWhiteSpace(sCultureName) == true)
                return false;

            if (sCultureName.Length != 5)
                return false;

            Span<char> cNormalizedCultureName = sCultureName.ToCharArray();

            if (cNormalizedCultureName[2] != '-')
                return false;

            cNormalizedCultureName[0] = char.ToLowerInvariant(cNormalizedCultureName[0]);
            cNormalizedCultureName[1] = char.ToLowerInvariant(cNormalizedCultureName[1]);
            cNormalizedCultureName[2] = '-';
            cNormalizedCultureName[3] = char.ToUpperInvariant(cNormalizedCultureName[3]);
            cNormalizedCultureName[4] = char.ToUpperInvariant(cNormalizedCultureName[4]);

            string sNormalizedCultureName = cNormalizedCultureName.ToString();

            if (this.sCultureName != sNormalizedCultureName)
                ResetSecurityStamp();

            this.sCultureName = sNormalizedCultureName;

            return true;
        }

        public bool TryChangeEmailAddress(string sEmailAddress, IPassportSetting ppSetting)
        {
            if (EmailAddressIsValid(sEmailAddress, ppSetting) == false)
                return false;

            if (this.EmailAddress != sEmailAddress)
            {
				this.bEmailAddressIsConfirmed = false;
				ResetSecurityStamp();
            }

            this.sEmailAddress = sEmailAddress;

            return true;
        }

        public bool TryChangePhoneNumber(string sPhoneNumber, IPassportSetting ppSetting)
        {
            if (PhoneNumberIsValid(sPhoneNumber, ppSetting) == false)
                return false;

            if (this.sPhoneNumber != sPhoneNumber)
            {
				this.bPhoneNumberIsConfirmed = false;
				ResetSecurityStamp();
            }

            this.sPhoneNumber = sPhoneNumber;

            return true;
        }

        public bool TryConfirmEmailAddress(string sEmailAddressToConfirm, IPassportSetting ppSetting)
        {
            if (EmailAddressIsValid(sEmailAddressToConfirm, ppSetting) == false)
                return false;

            if (sEmailAddressToConfirm == sEmailAddress)
            {
                this.bEmailAddressIsConfirmed = true;
                ResetSecurityStamp();
            }

            return true;
        }

        public bool TryConfirmPhoneNumber(string sPhoneNumberToConfirm, IPassportSetting ppSetting)
        {
            if (PhoneNumberIsValid(sPhoneNumberToConfirm, ppSetting) == false)
                return false;

            if (sPhoneNumberToConfirm == sPhoneNumber)
            {
                bPhoneNumberIsConfirmed = true;
                ResetSecurityStamp();
            }

            return true;
        }

        // The SecurityStamp property is automatically updated whenever a user's password is changed or when the user's account is modified in any way.
        private void ResetSecurityStamp()
        {
            sSecurityStamp = Guid.NewGuid().ToString();
        }

        private bool EmailAddressIsValid(string sEmailAddress, IPassportSetting ppSetting)
        {
            if (string.IsNullOrWhiteSpace(sEmailAddress) == true)
                return false;

            Span<char> cNormalizedEmailAddress = sEmailAddress.ToCharArray();

            int iAtSignIndex = cNormalizedEmailAddress.IndexOf('@');

            if (iAtSignIndex == -1)
                return false;

            int iDotIndex = cNormalizedEmailAddress.LastIndexOf('.');

            if (iDotIndex < iAtSignIndex)
                return false;

            return true;
        }

        private bool PhoneNumberIsValid(string sPhoneNumber, IPassportSetting ppSetting)
        {
            if (string.IsNullOrWhiteSpace(sPhoneNumber) == true)
                return false;

            if (sPhoneNumber.Length < ppSetting.MinimalPhoneNumberLength)
                return false;

            return true;
        }

		private static IPassportHolder? Initialize(
			string sConcurrencyStamp,
			string sCultureName,
			string sEmailAddress,
			bool bEmailAddressIsConfirmed,
			string sFirstName,
			Guid guId,
			string sLastName,
			string sPhoneNumber,
			bool bPhoneNumberIsConfirmed,
			string sSecurityStamp,
			IPassportSetting ppSetting)
		{
			IPassportHolder ppHolder = new PassportHolder(
				sConcurrencyStamp: sConcurrencyStamp,
				sCultureName: sCultureName,
				sEmailAddress: sEmailAddress,
				bEmailAddressIsConfirmed: bEmailAddressIsConfirmed,
				sFirstName: sFirstName,
				guId: guId,
				sLastName: sLastName,
				sPhoneNumber: sPhoneNumber,
				bPhoneNumberIsConfirmed: bPhoneNumberIsConfirmed,
				sSecurityStamp: sSecurityStamp);

			if (ppHolder.TryChangeCultureName(sCultureName) == false)
				return null;

			if (ppHolder.TryChangeEmailAddress(sEmailAddress, ppSetting) == false)
				return null;

			if (ppHolder.TryChangePhoneNumber(sPhoneNumber, ppSetting) == false)
				return null;

			return ppHolder;
		}

		public static IPassportHolder? Create(
            string sCultureName,
            string sEmailAddress,
            string sFirstName,
            string sLastName,
            string sPhoneNumber,
            IPassportSetting ppSetting)
        {
            return Initialize(
                sConcurrencyStamp: Guid.NewGuid().ToString(),
                sCultureName: sCultureName,
                sEmailAddress: sEmailAddress,
                bEmailAddressIsConfirmed: false,
                sFirstName: sFirstName,
                guId: Guid.NewGuid(),
                sLastName: sLastName,
                sPhoneNumber: sPhoneNumber,
                bPhoneNumberIsConfirmed: false,
                sSecurityStamp: Guid.NewGuid().ToString(),
                ppSetting: ppSetting);
        }

        public static IPassportHolder? Initialize(IPassportHolderTransferObject dtoPassportHolder, IPassportSetting ppSetting)
        {
            return Initialize(
                    sConcurrencyStamp: dtoPassportHolder.ConcurrencyStamp,
                    sCultureName: dtoPassportHolder.CultureName,
                    sEmailAddress: dtoPassportHolder.EmailAddress,
                    bEmailAddressIsConfirmed: dtoPassportHolder.EmailAddressIsConfirmed,
                    sFirstName: dtoPassportHolder.FirstName,
                    guId: dtoPassportHolder.Id,
                    sLastName: dtoPassportHolder.LastName,
                    sPhoneNumber: dtoPassportHolder.PhoneNumber,
                    bPhoneNumberIsConfirmed: dtoPassportHolder.PhoneNumberIsConfirmed,
                    sSecurityStamp: dtoPassportHolder.SecurityStamp,
                    ppSetting: ppSetting);
        }

        public IPassportHolderTransferObject WriteTo<T>() where T : IPassportHolderTransferObject, new()
        {
            return new T()
            {
                ConcurrencyStamp = sConcurrencyStamp,
                CultureName = sCultureName,
                EmailAddress = sEmailAddress,
                EmailAddressIsConfirmed = bEmailAddressIsConfirmed,
                FirstName = sFirstName,
                Id = guId,
                LastName = sLastName,
                PhoneNumber = sPhoneNumber,
                PhoneNumberIsConfirmed = bPhoneNumberIsConfirmed,
                SecurityStamp = sSecurityStamp
            };
        }
    }
}