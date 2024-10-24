using System;

namespace AgoraChat
{
	/**
	* The callback for a method execution failure.
	*
	* @param code	   The error code.
	* @param desc      The error description.
	*/
	public delegate void OnError(int code, string desc);

	/**
	* The callback for the method execution progress.
	*
	* @param progress   The execution progress value that ranges from 0 to 100 in percentage.
	* 
	*/
	public delegate void OnProgress(int progress);


	/**
	    * The class of callbacks without a return value.
		* 
	    */
	public class CallBack
	{
		/**
	    * The success callback.
		* 
	    */
		public Action Success;
		/**
	    * The error callback.
		* 
	    */
		public OnError Error;
		/**
	    * The progress callback.
		* 
	    */
		public OnProgress Progress;

		internal string callbackId;

		/**
	    * The result callback constructor.
	    *
	    * @param onSuccess      The success callback.
	    * @param onProgress     The progress callback.
	    * @param onError        The error callback.
	    * 
	    */
		public CallBack(Action onSuccess = null, OnProgress onProgress = null, OnError onError = null)
		{
			Success = onSuccess;
			Error = onError;
			Progress = onProgress;
		}
	}

	/**
	* The class of callbacks with a return value.
	*/
	public class ValueCallBack<T> : CallBack
	{
		/**
		 * The success callback with a return value.
		 */
		public Action<T> OnSuccessValue;

		/**
	    * The constructor for the class of callbacks with a return value.
	    *
	    * @param onSuccess      The success callback.
	    * @param onError        The error callback.
	    * 
	    */
		public ValueCallBack(Action<T> onSuccess = null, OnError onError = null)
		{
			OnSuccessValue = onSuccess;
			Error = onError;
		}
	}
}
