﻿
namespace Khrussk.Tests.Sockets {
	using System;
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>Connections test.</summary>
	[TestClass] public class ConnectionsTests {
		readonly SocketTestContext _context = new SocketTestContext();
		
		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_context.ListenerSocket.Listen(_context.EndPoint);
			_context.ClientSocket.Connect(_context.EndPoint);

			Assert.IsTrue(_context.WaitFor(() => _context.AcceptedSockets.Count() == 1, 1000));
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			_context.Cleanup();
		}

		/// <summary>Connection between sockets should be established.</summary>
		[TestMethod] public void ConnectionShouldBeEstablishedTest() {
			Assert.AreEqual(1, _context.AcceptedSockets.Count());
		}

		/// <summary>Connection should be closed after disconnection on local site.</summary>
		[TestMethod] public void ConnectionShouldBeClosedAfterDisconnectionOnLocalSiteTest() {
			_context.ClientSocket.Disconnect();
			Assert.IsTrue(_context.WaitFor(() => _context.ClientSockets.Count() == 0, 1000));
		}

		/// <summary>Connection should be closed after disconnection on remote site.</summary>
		[TestMethod] public void ConnectionShouldBeClosedAfterDisconnectionOnRemoteSiteTest() {
			_context.AcceptedSockets.First().Disconnect();
			Assert.IsTrue(_context.WaitFor(() => _context.AcceptedSockets.Count() == 0, 1000));
		}

		/// <summary>Socket can be disconnected serveral times.</summary>
		[TestMethod] public void ConnectionCanBeClosedSeveralTimesTest() {
			_context.ClientSocket.Disconnect();
			_context.ClientSocket.Disconnect();
			Assert.IsTrue(_context.WaitFor(() => _context.ClientSockets.Count() == 0, 1000));
		}

		/// <summary>Listener can handle multiple connections.</summary>
		[TestMethod] public void ListenerCanHandleSeveralConnectionsTest() {
			_context.NewSocket().Connect(_context.EndPoint);
			_context.NewSocket().Connect(_context.EndPoint);
			_context.NewSocket().Connect(_context.EndPoint);
			
			_context.WaitFor(() => _context.AcceptedSockets.Count() == 3 + 1 /* one connected before */, 1000);
		}

		/// <summary>Can not connect twice.</summary>
		[TestMethod, ExpectedException(typeof(InvalidOperationException))]
		public void ConnectionCanNotBeEstablishedSeveralTimesTest() {
			_context.ClientSocket.Connect(_context.EndPoint);
		}

		/// <summary>Recconnection works properly.</summary>
		[TestMethod] public void ConnectionCanBeReusedTest() {
			var socket = _context.ClientSocket;
			socket.Disconnect();
			Assert.IsTrue(_context.WaitFor(() => _context.ClientSockets.Count() == 0, 1000));
			socket.Connect(_context.EndPoint);
			Assert.IsTrue(_context.WaitFor(() => _context.ClientSockets.Count() == 1, 1000));
		}

		/// <summary>Can not listen twice.</summary>
		[TestMethod, ExpectedException(typeof(InvalidOperationException))]
		public void CanNotListenSeveralTimes() {
			_context.ListenerSocket.Listen(_context.EndPoint);
		}
	}
}