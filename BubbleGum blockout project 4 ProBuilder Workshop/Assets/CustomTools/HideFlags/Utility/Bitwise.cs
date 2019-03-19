using UnityEngine;

public static class Bitwise
{
	// Checks if a bitwise element contains at least one of different specific bitwise elements.
	public static bool Contains( int _elementToCheck, params int[] _comparator )
	{
		Debug.Assert( _comparator.Length > 0 );

		for( int i = 0; i < _comparator.Length; i++ )
		{
			if( _comparator[i] < 0 )
			{
				break;
			}
			if( (_comparator[i] & _elementToCheck) >= _comparator[i] )
			{
				return true;
			}
		}

		return false;
	}

	// Sums up all bitwise elements
	public static int Add( params int[] _elements ) {
		int _bitwise = 0;

		for( int i = 0; i < _elements.Length; i++ )
		{
			_bitwise = _bitwise | _elements[i];
		}

		return _bitwise;
	}

	// Substracts bitwise elements from original
	public static int Substract( int _original, params int[] _elements ) {
		int _bitwise = 0;

		for( int i = 0; i < _elements.Length; i++ )
		{
			_bitwise = _bitwise | _elements[i];
		}

		return _original ^ _bitwise;
	}

	// Substracts elements that are already contained and adds elements that aren't
	public static int Toggle( int _original, params int[] _elements )
	{
		for( int i = 0; i < _elements.Length; i++ )
		{
			if( Bitwise.Contains( _original, _elements[i] ) )
			{
				_original = Bitwise.Substract( _original, _elements[i] );
			}
			else
			{
				_original = Bitwise.Add( _original, _elements[i] );
			}
		}

		return _original;
	}
}