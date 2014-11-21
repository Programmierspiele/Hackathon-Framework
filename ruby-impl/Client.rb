require_relative 'Network'
require_relative 'GameObject'

class Client
  def initialize (host, port, ssl = false,  cert_folder = '.')
    @network = Network.new
    @network.connect_no_ssl host, port unless ssl
    @network.connect host, port, cert_folder if ssl
  end

  def send_command(command)
    write(:command => command)
  end

  def send_speed(speed)
    write(:speed => speed)
  end

  def send_rotation(rotation)
    write(:rotation => rotation)
  end

  def send_name(name)
    write(:name => name)
  end

  def send_ping
    write(:ping => 'ping')
  end

  def write(map)
    @network.send map
  end

  def read
    json = @network.read

    scene = json['scene'] || Array.new
    command = json['command'] || nil
    speed = json['speed'] || nil
    rotation = json['rotation'] || nil

    tmp = Array.new
    scene.each do |q|
      tmp << GameObject.new(q['name'], q['x'], q['y'], q['z'], q['rotation'], q['extra'])
    end
    scene = tmp

    return scene, command, speed, rotation
  end

  def read_map
    json = @network.read
    json['scene'] ||= Array.new
    tmp = Array.new
    json['scene'].each do |q|
      tmp << GameObject.new(q['name'], q['x'], q['y'], q['z'], q['rotation'], q['extra'])
    end
    json['scene'] = tmp
    return json
  end
end

if __FILE__ == $0
  def client(id)
    client = Client.new 'localhost', 25555

    client.send_name(id)
    puts client.read_map

    while true
      sleep 1
      client.send_ping
      map = client.read_map
      puts map
    end
  end

  client('ken1')

  puts 'Disconnected'
end