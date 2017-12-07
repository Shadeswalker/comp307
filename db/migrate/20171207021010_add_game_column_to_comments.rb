class AddGameColumnToComments < ActiveRecord::Migration[5.1]
  def change
    add_column :comments, :game, :integer
  end
end
