class CreateComments < ActiveRecord::Migration[5.1]
  def change
    create_table :comments do |t|
      t.text :text
      t.string :author, default: 'Anonymous'
      t.integer :reply

      t.timestamps
    end
  end
end
