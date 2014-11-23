#
# Author: Michael Fürst
# Version: 1.0
#

require 'socket'
require_relative 'Connection'
# A worker. Dial home to receive ask for tasks.
class Network

  # Initialize the worker.
  def initialize
  end

  def self.connect(host, port)
    socket = TCPSocket.open host, port
    socket.setsockopt Socket::IPPROTO_TCP, Socket::TCP_NODELAY, 1
    socket
  end

  def start_listen(port, game_manager, timeout)
    server = TCPServer.open(port)
    @thread = Thread.start do
      log "Listening unsecured on port #{port}"
      accept_clients game_manager, server, timeout
    end
  end

  def accept_clients(game_manager, server, timeout)
    loop do
      Thread.start(server.accept) do |socket|
        log 'Client connected.!'
        begin
          game_manager.connection_started(Connection.new socket, game_manager, timeout)
        rescue => e
          puts e.backtrace
          unless socket.closed?
            disconnect socket
          end
        end
        log 'Client disconnected!'
      end
    end
  end

  def end_listen
    @thread.kill
  end

  def log (msg)
    puts '[' + Time.new.strftime('%Y-%m-%d %H:%M:%S') + '] ' + msg
  end
end