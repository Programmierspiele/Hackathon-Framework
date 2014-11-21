class GameObject
  attr_reader :name, :x, :y, :z, :rotation, :extra

  def initialize (name, x, y, z, rotation, extra)
    @name = name || ''
    @x = x || 0
    @y = y || 0
    @z = z || 0
    @rotation = rotation || 0
    @extra = extra || Array.new
  end
end