class CreateGames < ActiveRecord::Migration[5.1]
  def change
    create_table :games do |t|
      t.string :name
      t.string :dev
      t.integer :fund_goal
      t.integer :current_fund
      t.integer :nmb_backers
      t.integer :funding_period

      t.timestamps
    end
  end
end
