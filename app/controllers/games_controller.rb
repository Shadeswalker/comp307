class GamesController < ApplicationController
  before_action :find_game, only: [:play_game]
  before_action :retrieve_comments, only: [:play_game]

  def index
    @games = Game.all
  end

  def play_game
    if !(@game.fund_goal)
      redirect_to '/games/coming_soon'
    end
  end

  def add_comment
    @comment = Comment.new(comment_params)
    debugger
    @comment.save
    if @comment.reply == nil
      Comment.update(@comment.id, :reply => @comment.id)
    end
    redirect_to "/play_game/#{@comment.game}"
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def find_game
      @game = Game.find(params[:id])
    end

    def retrieve_comments
      @comments = Comment.where(game: params[:id]).order(:reply, :created_at)

    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def comment_params
      params.require(:comment).permit(:text, :game)
    end
end
