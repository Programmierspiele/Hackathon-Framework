#
# Author: Michael Fürst
# Version: 1.0
#

require 'socket'
require 'openssl'
require 'json'
# A worker. Dial home to receive ask for tasks.
class Network

  class Connection
    def initialize(socket, network)
      @socket = socket
      @network = network
    end
    def on_message_handler_loop on_message_handler
      begin
        until closed? do
          on_message_handler.call read
        end
      rescue => e
        puts e.backtrace
      end
    end
    def send msg
      @network.send msg, @socket unless closed?
    end
    def read
      @network.read @socket unless closed?
    end
    def close
      @network.close @socket unless closed?
    end
    def closed?
      @socket.closed?
    end
  end

  attr_reader :socket
# Initialize the worker.
  def initialize
  end

  def connect(host, port, folder)
    cert_file = File.open(folder + '/cert.pem')
    socket = TCPSocket.open host, port
    socket.setsockopt Socket::IPPROTO_TCP, Socket::TCP_NODELAY, 1
    expectedCert = OpenSSL::X509::Certificate.new(cert_file)
    ssl = OpenSSL::SSL::SSLSocket.new(socket)
    ssl.sync_close = true
    ssl.connect
    if ssl.peer_cert.to_s != expectedCert.to_s
      log 'Unexpected certificate'
      return nil
    end
    @socket = ssl
  end
  def connect_no_ssl(host, port)
    socket = TCPSocket.open host, port
    socket.setsockopt Socket::IPPROTO_TCP, Socket::TCP_NODELAY, 1
    @socket = socket
  end

  def create_certificate(private_path = 'private.pem', cert_path = 'cert.pem',
                         country = 'DE', organization = 'none', organization_unit = 'none', common_name = 'botnet_v2')
    key = OpenSSL::PKey::RSA.new(1024)
    public_key = key.public_key
    subject = '/C='+country+'/O='+organization+'/OU='+organization_unit+'/CN='+common_name
    cert = OpenSSL::X509::Certificate.new
    cert.subject = cert.issuer = OpenSSL::X509::Name.parse(subject)
    cert.not_before = Time.now
    cert.not_after = Time.now + 365 * 24 * 60 * 60
    cert.public_key = public_key
    cert.serial = 0x0
    cert.version = 2
    ef = OpenSSL::X509::ExtensionFactory.new
    ef.subject_certificate = cert
    ef.issuer_certificate = cert
    cert.extensions = [
        ef.create_extension('basicConstraints','CA:TRUE', true),
        ef.create_extension('subjectKeyIdentifier', 'hash')
    ]
    cert.add_extension ef.create_extension('authorityKeyIdentifier', 'keyid:always,issuer:always')
    cert.sign key, OpenSSL::Digest::SHA1.new
    open (private_path), 'w' do |io|
      io.write key
    end
    open (cert_path), 'w' do |io|
      io.write cert.to_pem
    end
  end

  def listen(on_connect_handler, port, folder)
    private_path = folder + '/private.pem'
    public_path = folder + '/cert.pem'
    unless File.exists? private_path
      create_certificate private_path, public_path
    end
    cert_pem = File.open(public_path)
    private_pem = File.open(private_path)
    server = TCPServer.open(port)
    sslContext = OpenSSL::SSL::SSLContext.new
    sslContext.cert = OpenSSL::X509::Certificate.new(cert_pem)
    sslContext.key = OpenSSL::PKey::RSA.new(private_pem)
    sslServer = OpenSSL::SSL::SSLServer.new(server, sslContext)
    thread = Thread.start do
      log "Listening secured on port #{port}"
      accept_clients on_connect_handler, sslServer
    end
    thread
  end

  def listen_no_ssl(on_connect_handler, port)
    server = TCPServer.open(port)
    thread = Thread.start do
      log "Listening unsecured on port #{port}"
      accept_clients on_connect_handler, server
    end
    thread
  end

  def accept_clients(on_connect_handler, server)
    loop do
      Thread.start(server.accept) do |socket|
        log 'Client connected.!'
        begin
          on_connect_handler.call(Connection.new socket, self)
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

  def send(message, socket = @socket)
    socket.puts(JSON.generate(message)) unless socket.closed?
  end

  def read(socket = @socket)
    res = socket.gets
    res = res.chomp if res
    res = JSON.parse(res) if res
    res
  end

  def close(socket = @socket)
    socket.close unless socket.closed?
  end

  def log (msg)
    puts '[' + Time.new.strftime('%Y-%m-%d %H:%M:%S') + '] ' + msg
  end
end