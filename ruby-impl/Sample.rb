require_relative 'hackathonlib/Client'
require_relative 'hackathonlib/Connection'
require_relative 'hackathonlib/Network'

class Sample < Client
  def update_scene(scene)

  end

  def get_command
    nil
  end

  def get_name
    'Sample'
  end

  def get_speed
    nil
  end

  def get_rotation
    nil
  end

  def do_ping
    true
  end
end

if __FILE__ == $0
  # Define hostname and port.
  hostname = 'localhost'
  port = 25555

  # Open up a tcp connection.
  client = Network.connect(hostname, port)
  connection = Connection.new(client, Sample.new, -1)

  # Start the client loop.
  connection.start_client_loop

  # Join the connection since it's callback based.
  connection.join
end