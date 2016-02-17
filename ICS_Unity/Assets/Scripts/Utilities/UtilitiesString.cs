using UnityEngine;
using System;
using System.Text.RegularExpressions;
using SystemPath = System.IO.Path;
using UObject = UnityEngine.Object;

public static class UtilitiesString
{
	public static string RemoveExtension(this string inString)
	{
		return inString.RemoveSuffix(SystemPath.GetExtension(inString));
	}

	public static string RemoveSuffix(this string inString, string inSuffix)
    {
		string trimmedString = inString;
        if (trimmedString.EndsWith(inSuffix))
            trimmedString = trimmedString.Substring(0, trimmedString.Length - inSuffix.Length);
        return trimmedString;
    }

	public static string RemoveUpToAndIncluding(this string inString, string inToken)
	{
		int atSymbolIndex = inString.IndexOf(inToken);
		int targetIndex = atSymbolIndex + inToken.Length;
		if(atSymbolIndex != -1 && targetIndex < inString.Length)
		{
			return inString.Substring(targetIndex);
		}
		return inString;
	}

	public static string RemoveUpToAndIncludingLast(this string inString, string inToken)
	{
		int atSymbolIndex = inString.LastIndexOf(inToken);
		int targetIndex = atSymbolIndex + inToken.Length;
		if(atSymbolIndex != -1 && targetIndex < inString.Length)
		{
			return inString.Substring(targetIndex);
		}
		return inString;
	}
}