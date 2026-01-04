using Godot;
using System;

public partial class Network : Node
{
	private MultiplayerApi _api;
	private ENetMultiplayerPeer _peer = new ENetMultiplayerPeer();
	
	public void JoinServer(string address, int port)
	{
		_peer.CreateClient("127.0.0.1", 8000);
		Multiplayer.MultiplayerPeer = _peer;
	}
	
	public void HostServer(int port, int maxClients)
	{
		_peer.CreateServer(8000, 5);
		Multiplayer.MultiplayerPeer = _peer;
	}
	
	public void TerminateNetworking()
	{
		_peer = null;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_api = GetTree().GetMultiplayer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
