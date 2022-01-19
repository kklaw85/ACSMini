﻿// See license at the bottom of the file
using System;
using System.Windows;
using System.Windows.Markup;

namespace WPFSharp.Globalizer
{
	//[TypeConverter(typeof(GlobalizedDynamicResourceExtensionConverter))]
	[MarkupExtensionReturnType( typeof( object ) )]
	public class GlobalizedResourceExtension : DynamicResourceExtension
	{
		private object _FallbackValue;

		#region Constructors

		/// <summary> 
		///  Default Contructor
		/// </summary>
		public GlobalizedResourceExtension()
		{
			this.FallbackValue = DependencyProperty.UnsetValue;
		}

		/// <summary> 
		///  Constructor that takes the resource key that this is a static reference to.
		/// </summary> 
		public GlobalizedResourceExtension( object inResourceKey )
		{
			this.ResourceKey = inResourceKey;
			this.FallbackValue = DependencyProperty.UnsetValue;
		}

		/// <summary> 
		///  Constructor that takes the resource key that this is a static reference to.
		/// </summary> 
		public GlobalizedResourceExtension( object inResourceKey, object inFallBackValue )
		{
			this.ResourceKey = inResourceKey;
			this.FallbackValue = inFallBackValue ?? DependencyProperty.UnsetValue;
		}

		#endregion

		#region Properties

		/// <summary>
		/// A value to use if the DynamicResource is not found.
		/// </summary>
		public object FallbackValue
		{
			get { return this._FallbackValue; }
			set { this._FallbackValue = value; }
		}

		/// <summary>
		/// A name used to group ResourceDictionaries
		/// </summary>
		public string GroupName { get; set; }

		/// <summary>
		/// A specified Style resource dictionary.
		/// If a resource dictionary is loaded, and this value is set, then the specified 
		/// style resource dictionary is loaded.
		/// </summary>
		public string LinkedStyle { get; set; }

		#endregion

		/// <summary>
		///  Return an object that should be set on the targetObject's targetProperty 
		///  for this markup extension.  For DynamicResourceExtension, this is the object found in
		///  a resource dictionary in the current parent chain that is keyed by ResourceKey 
		/// </summary> 
		/// <returns>
		///  The object to set on this property. 
		/// </returns>
		public override object ProvideValue( IServiceProvider inServiceProvider )
		{
			if ( this.ResourceKey == null || string.IsNullOrWhiteSpace( this.ResourceKey.ToString() ) )
				throw new InvalidOperationException( "ResourceKey cannot be null or empty." );

			if ( GlobalizedApplication.Instance == null )
				return this.FallbackValue;

			// Use FallbackValue if it exists by adding it to the FallbackResourceDictionary.
			if ( this.FallbackValue != null )
			{
				// Remove it in case it is already there
				GlobalizedApplication.Instance.FallbackResourceDictionary.Remove( this.ResourceKey );
				// Add it
				GlobalizedApplication.Instance.FallbackResourceDictionary.Add( this.ResourceKey, this.FallbackValue );
			}

			return base.ProvideValue( inServiceProvider );
		}

	}
}

#region License
/*
WPFSharp.Globalizer - A project deisgned to make localization and styling
                      easier by decoupling both process from the build.

Copyright (c) 2015, Jared Barneck (Rhyous)
All rights reserved.
 
Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
 
1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.
3. Use of the source code or binaries that in any way competes with WPFSharp.Globalizer
   or competes with distribution, whether open source or commercial, is 
   prohibited unless permission is specifically granted under a separate
   license by Jared Barneck (Rhyous).
4. Forking for personal or internal, or non-competing commercial use is allowed.
   Distributing compiled releases as part of your non-competing project is 
   allowed.
5. Public copies, or forks, of source is allowed, but from such, public
   distribution of compiled releases is forbidden.
6. Source code enhancements or additions are the property of the author until
   the source code is contributed to this project. By contributing the source
   code to this project, the author immediately grants all rights to the
   contributed source code to Jared Barneck (Rhyous).
 
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion
