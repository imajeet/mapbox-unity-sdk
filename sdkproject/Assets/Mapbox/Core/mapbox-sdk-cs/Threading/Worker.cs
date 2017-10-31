using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System;

namespace Mapbox.Threading
{

	/// <summary>
	/// <para>Don't forget to call Dispose() when you are done with the worker!</para>
	/// <para>Simple Worker class that uses BackgroundWorker under the hood to distribute load across processors.</para>
	/// </summary>
	public class Worker : IDisposable
	{

		//TODO: implement pool, or Queue or whatever
		//currently one Worker class has to be initialized per workload
		//long term goal: have one global Worker instance that takes workloads
		//and balances thems according to available processors
		//private BackgroundWorker[] _workerPool;

		private BackgroundWorker _worker;
		private bool _disposed;


		/// <summary>
		/// <para>Don't forget to call Dispose() when you are done with the worker!</para>
		/// <para>Simple Worker class that uses BackgroundWorker under the hood to distribute load across processors.</para>
		/// </summary>
		public Worker()
		{
			//_workerPool = new BackgroundWorker[System.Environment.ProcessorCount];

			_worker = new BackgroundWorker();
			_worker.WorkerSupportsCancellation = true;
			_worker.WorkerReportsProgress = true;
			_worker.DoWork += worker_DoWork;
		}


		#region IDisposable

		~Worker() { Dispose(false); }


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposeManagedResources)
		{
			if (!_disposed)
			{
				if (disposeManagedResources)
				{
					if (null != _worker)
					{
						if (_worker.IsBusy) { _worker.CancelAsync(); }
						_worker.DoWork -= worker_DoWork;
						_worker = null;
					}
				}

				_disposed = true;
			}
		}

		#endregion


		/// <summary>
		/// Action to be executed via BackgroundWorker.
		/// </summary>
		/// <param name="workLoad">Work load as an Action</param>
		public void ProcessWorkLoad(Action workLoad)
		{
			_worker.RunWorkerAsync(workLoad);
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{

			Action workload = e.Argument as Action;
			workload();
		}


	}
}