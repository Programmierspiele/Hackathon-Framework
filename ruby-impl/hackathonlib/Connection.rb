require 'socket'
require 'json'
require_relative 'GameObject'

class Connection
  def initialize(socket, game_manager, timeout)
    @socket = socket
    @game_manager = game_manager
    @timeout = timeout
    @stop = true
    @name = 'none'

    @thread = Thread.start do
      run
    end
  end

  def start_client_loop
    msg = Hash.new
    msg['ping'] = 'ping'
    write(msg)
  end

  def run
    @stop = false
    until @stop do
      begin
        write (handle read)
      rescue => e
        puts e.backtrace
      end
    end

    @socket.close unless @socket.closed?
  end

  def write(message, socket = @socket)
    socket.puts(JSON.generate(message)) unless socket.closed?
  end

  def read(socket = @socket)
    return if socket.closed?
    res = socket.gets
    res = res.chomp if res
    res = JSON.parse(res) if res
    res
  end

  def quit
     @stop = true
  end

  def join
    @thread.join
  end

  def handle msg
    result = Hash.new

    if msg['scene']
      tmp = Array.new
      msg['scene'].each do |q|
        tmp << GameObject.new(q['name'], q['x'], q['y'], q['z'], q['rotation'], q['extra'])
      end
      @game_manager.update_scene(tmp)
    end
    if msg['name'] && @name == 'none'
      @name = msg['name']
      @game_manager.connection_ready self
    end

    @game_manager.execute_command(msg['command']) if msg['command']
    @game_manager.change_speed(msg['speed']) if msg['speed']
    @game_manager.change_rotation(msg['rotation']) if msg['rotation']

    result['ping'] = 'pong' if msg['ping'] && msg['ping'] == 'ping'

    scene = @game_manager.get_scene
    result['scene'] = scene if scene != nil

    command = @game_manager.get_command
    result['command'] = command if command != nil

    name = @game_manager.get_name
    result['name'] = name if name != nil

    speed = @game_manager.get_speed
    result['speed'] = speed if speed != nil

    rotation = @game_manager.get_rotation
    result['rotation'] = rotation if rotation != nil

    result['ping'] = 'ping' if @game_manager.do_ping && msg['ping'] != 'ping'

    result
  end
end