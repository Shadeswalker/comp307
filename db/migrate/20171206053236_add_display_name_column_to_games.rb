class AddDisplayNameColumnToGames < ActiveRecord::Migration[5.1]
  def change
    add_column :games, :display_name, :text
  end
end
