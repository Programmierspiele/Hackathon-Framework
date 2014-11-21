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

  def write(object)
    @network.send object
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
  client = Client.new 'localhost', 25555

  client.write(:ping => 'ping')
  scene, command, speed, rotation = client.read

  puts scene
  puts command
  puts speed
  puts rotation

  client.write(:ping => 'ping')
  puts client.read_map
end