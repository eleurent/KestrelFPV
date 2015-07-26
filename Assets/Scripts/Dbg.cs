#define FINAL
using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

public class Dbg : MonoBehaviour
{
	//-------------------------------------------------------------------------------------------------------------------------
	public		string			LogFile = "log.txt";
	public		bool			EchoToConsole = true;
	public		bool			AddTimeStamp = true;

	//-------------------------------------------------------------------------------------------------------------------------
	private		StreamWriter	OutputStream;

	//-------------------------------------------------------------------------------------------------------------------------
	static Dbg Singleton = null;

	//-------------------------------------------------------------------------------------------------------------------------
	public static Dbg Instance
	{
		get { return Singleton; }
	}

	//-------------------------------------------------------------------------------------------------------------------------
	void Awake()
	{

		if (Singleton != null)
		{
			UnityEngine.Debug.LogError("Multiple Dbg Singletons exist!");
			return;
		}

		Singleton = this;

#if !FINAL
		// Open the log file to append the new log to it.
		OutputStream = new StreamWriter( LogFile, true );
#endif
	}

	//-------------------------------------------------------------------------------------------------------------------------
	void OnDestroy()
	{
#if !FINAL
		if ( OutputStream != null )
		{
			OutputStream.Close();
			OutputStream = null;
		}
#endif
	}

	//-------------------------------------------------------------------------------------------------------------------------
	private void Write( string message )
	{
#if !FINAL
		if ( AddTimeStamp )
		{
			DateTime now = DateTime.Now;
//			message = string.Format("[{0:H:mm:ss}] {1}", now, message );
			message = string.Format("{0:ss.ffff}, {1}", now, message );
		}

		if ( OutputStream != null )
		{
				
			OutputStream.WriteLine( message );
			OutputStream.Flush();
		}

		if ( EchoToConsole )
		{
			UnityEngine.Debug.Log( message );
		}
#endif
	}

	//-------------------------------------------------------------------------------------------------------------------------
//	[Conditional("DEBUG"), Conditional("PROFILE")]
	public static void Trace( string Message)
	{
#if !FINAL
		if ( Dbg.Instance != null )
			Dbg.Instance.Write( Message );
		else
			// Fallback if the debugging system hasn't been initialized yet.
			UnityEngine.Debug.Log( Message );
#endif
	}


}
