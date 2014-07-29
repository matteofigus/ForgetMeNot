using System;
using Newtonsoft;
using FluentValidation.Validators;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ReminderService.API.HTTP
{
	public class IsValidJsonValidator : PropertyValidator
	{
		public IsValidJsonValidator ()
			: base ("Propery {PropertyName} is not valid JSON!")
		{
			//empty
		}

		protected override bool IsValid (PropertyValidatorContext context)
		{
			//hmm - using exceptions to enforce validation. but it is the only way i can think to do this without writing my own parser
			try {
				var jsonString = Encoding.UTF8.GetString((context.PropertyValue as byte[]));
				JContainer.Parse (jsonString);
				return true;
			} catch (Exception) {
				return false;
			}
		}
	}
}

